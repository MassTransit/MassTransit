// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.SystemView.Core.Consumer
{
    using System;
    using Distributor.Messages;
    using Magnum;
    using Services.Subscriptions.Messages;
    using Transports.Msmq;
    using ViewModel;

    public class SubscriptionDataConsumer :
        Consumes<SubscriptionRefresh>.All,
        Consumes<AddSubscription>.All,
        Consumes<RemoveSubscription>.All,
        Consumes<IWorkerAvailable>.All,
        IDisposable
    {
        private IServiceBus _bus;
        private readonly Guid _clientId = CombGuid.Generate();
        private IEndpoint _subscriptionServiceEndpoint;
        private UnsubscribeAction _unsubscribe;
        private readonly IObjectBuilder _objectBuilder;

        public SubscriptionDataConsumer(IObjectBuilder objectBuilder)
        {
            _objectBuilder = objectBuilder;

            BootstrapServiceBus();

            ConnectToSubscriptionService();
        }

        private void ConnectToSubscriptionService()
        {
            _subscriptionServiceEndpoint = _objectBuilder.GetInstance<IEndpointFactory>()
                .GetEndpoint(_objectBuilder.GetInstance<IConfiguration>().SubscriptionServiceUri);

            _subscriptionServiceEndpoint.Send(new AddSubscriptionClient(_clientId, _bus.Endpoint.Uri, _bus.Endpoint.Uri));
        }

        private void BootstrapServiceBus()
        {
            MsmqEndpointConfigurator.Defaults(x =>
                {
                    x.CreateMissingQueues = true;
                });

            _bus = _objectBuilder.GetInstance<IServiceBus>();
            _unsubscribe = _bus.Subscribe(this);
        }

        public void Consume(SubscriptionRefresh message)
        {
            LocalSubscriptionCache.Endpoints.Update(message.Subscriptions);
        }

        public void Consume(AddSubscription message)
        {
            LocalSubscriptionCache.Endpoints.Update(message.Subscription);
        }

        public void Consume(RemoveSubscription message)
        {
            LocalSubscriptionCache.Endpoints.Remove(message.Subscription.EndpointUri, message.Subscription.MessageName);
        }

        public void Consume(IWorkerAvailable message)
        {
            LocalSubscriptionCache.Endpoints.Update(message);
        }

        public void UpdateWorker(Uri controlUri, string type, int pendingLimit, int inProgressLimit)
        {
            var endpoint = _objectBuilder.GetInstance<IEndpointFactory>().GetEndpoint(controlUri);

            endpoint.Send(new ConfigureWorker() { InProgressLimit = inProgressLimit, MessageType = type, PendingLimit = pendingLimit });
        }

        public void RemoveSubscription(Guid clientId, string messageName, string correlationId, Uri endpointUri)
        {
            _bus.Publish(new RemoveSubscription(new SubscriptionInformation(clientId, 0, messageName, correlationId, endpointUri)));
        }

        public void Dispose()
        {
            _unsubscribe();
        }
    }
}