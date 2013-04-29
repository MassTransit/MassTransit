// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Services.Subscriptions.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Logging;
    using Magnum.Extensions;
    using Messages;
    using Saga;
    using Subscriptions.Messages;

    public class SubscriptionService :
        Consumes<SubscriptionClientAdded>.All,
        Consumes<SubscriptionClientRemoved>.All,
        Consumes<SubscriptionAdded>.All,
        Consumes<SubscriptionRemoved>.All,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<SubscriptionService>();
        readonly ISagaRepository<SubscriptionClientSaga> _subscriptionClientSagas;
        readonly ISagaRepository<SubscriptionSaga> _subscriptionSagas;
        IServiceBus _bus;
        bool _disposed;
        UnsubscribeAction _unsubscribeToken = () => false;

        public SubscriptionService(IServiceBus bus,
                                   ISagaRepository<SubscriptionSaga> subscriptionSagas,
                                   ISagaRepository<SubscriptionClientSaga> subscriptionClientSagas)
        {
            _bus = bus;
            _subscriptionSagas = subscriptionSagas;
            _subscriptionClientSagas = subscriptionClientSagas;
        }

        public void Consume(SubscriptionAdded message)
        {
            _log.Debug(() => string.Format("Subscription Added: {0} [{1}]", message.Subscription, message.Subscription.CorrelationId));

            var add = new AddSubscription(message.Subscription);

            SendToClients(add);
        }

        public void Consume(SubscriptionClientAdded message)
        {
            _log.Debug(() => string.Format("Subscription Client Added: {0} [{1}]", message.ControlUri, message.ClientId));

            var add = new AddSubscriptionClient(message.ClientId, message.ControlUri, message.DataUri);
            SendClientToClients(add);

            SendCacheUpdateToClient(message.ControlUri, message.ClientId);
        }

        public void Consume(SubscriptionClientRemoved message)
        {
            _log.Debug(() => string.Format("Subscription Client Removed: {0} [{1}]", message.ControlUri, message.CorrelationId));

            var remove = new RemoveSubscriptionClient(message.CorrelationId, message.ControlUri, message.DataUri);
            SendClientToClients(remove);
        }

        public void Consume(SubscriptionRemoved message)
        {
            _log.Debug(() => string.Format("Subscription Removed: {0} [{1}]", message.Subscription, message.Subscription.CorrelationId));

            var remove = new RemoveSubscription(message.Subscription);

            SendToClients(remove);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Start()
        {
            _log.InfoFormat("Subscription Service Starting: {0}", _bus.Endpoint.Address);
            _unsubscribeToken += _bus.SubscribeInstance(this);

            _unsubscribeToken += _bus.SubscribeSaga(_subscriptionClientSagas);
            _unsubscribeToken += _bus.SubscribeSaga(_subscriptionSagas);

            _log.Info("Subscription Service Started");
        }

        public void Stop()
        {
            _log.Info("Subscription Service Stopping");

            _unsubscribeToken();

            _log.Info("Subscription Service Stopped");
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                try
                {
                    _bus.Dispose();
                    _bus = null;
                }
                catch (Exception ex)
                {
                    string message = "Error in shutting down the SubscriptionService: " + ex.Message;
                    var exp = new ShutDownException(message, ex);
                    _log.Error(message, exp);
                    throw exp;
                }
            }

            _disposed = true;
        }

        void SendToClients<T>(T message)
            where T : SubscriptionChange
        {
            SubscriptionClientSaga forClient = _subscriptionClientSagas
                .Where(x => x.CorrelationId == message.Subscription.ClientId)
                .FirstOrDefault();

            if (forClient == null)
                return;

            List<SubscriptionClientSaga> sagas = _subscriptionClientSagas
                .Where(x => x.CurrentState == SubscriptionClientSaga.Active)
                .Where(x => x.ControlUri != forClient.ControlUri)
                .ToList();

            _log.DebugFormat("Sending {0}:{1} to {2} clients", typeof (T).Name, message.Subscription.MessageName, sagas.Count());
            sagas.Each(client =>
                {
                    IEndpoint endpoint = _bus.GetEndpoint(client.ControlUri);

                    endpoint.Send(message, x => x.SetSourceAddress(_bus.Endpoint.Address.Uri));
                });
        }

        void SendClientToClients<T>(T message)
            where T : SubscriptionClientMessageBase
        {
            IEnumerable<SubscriptionClientSaga> sagas = _subscriptionClientSagas
                .Where(x => x.CurrentState == SubscriptionClientSaga.Active)
                .Where(x => x.ControlUri != message.ControlUri);

            sagas.Each(client =>
                {
                    _log.DebugFormat("Sending {2} {0} to {1}", message.CorrelationId, client.ControlUri, typeof (T).Name);

                    IEndpoint endpoint = _bus.GetEndpoint(client.ControlUri);

                    endpoint.Send(message, x => x.SetSourceAddress(_bus.Endpoint.Address.Uri));
                });
        }

        void SendCacheUpdateToClient(Uri uri, Guid clientId)
        {
            IEndpoint endpoint = _bus.GetEndpoint(uri);

            IEnumerable<SubscriptionClientSaga> sagas = _subscriptionClientSagas
                .Where(x => x.CurrentState == SubscriptionClientSaga.Active && x.ControlUri != uri)
                .ToList();

            sagas.Each(client =>
                {
                    _log.DebugFormat("Sending AddClient {0} to {1}", client.CorrelationId, uri);

                    var message = new AddSubscriptionClient(client.CorrelationId, client.ControlUri, client.DataUri);

                    endpoint.Send(message, x => x.SetSourceAddress(_bus.Endpoint.Address.Uri));
                });

            List<Guid> clients = sagas.Select(x => x.CorrelationId).ToList();

            SubscriptionInformation[] subscriptions = _subscriptionSagas
                .Where(x => x.CurrentState == SubscriptionSaga.Active && clients.Contains(x.SubscriptionInfo.ClientId))
                .Select(x => x.SubscriptionInfo).ToArray();

            _log.InfoFormat("Sending {0} subscriptions to {1}", subscriptions.Length, uri);

            var response = new SubscriptionRefresh(subscriptions);

            endpoint.Send(response, x => x.SetSourceAddress(_bus.Endpoint.Address.Uri));
        }
    }
}