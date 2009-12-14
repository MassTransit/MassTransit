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
namespace MassTransit.SystemView.Consumer
{
    using System;
    using Magnum;
    using Distributor.Messages;
    using Services.HealthMonitoring.Messages;
    using Services.Subscriptions.Messages;
    using Services.Timeout.Messages;
    using ViewModel;
    using Transports.Msmq;
    using StructureMap.Attributes;

    public class SubscriptionDataConsumer :
        Consumes<SubscriptionRefresh>.All,
        Consumes<AddSubscription>.All,
        Consumes<RemoveSubscription>.All,
        Consumes<HealthUpdate>.All,
        Consumes<TimeoutScheduled>.All,
        Consumes<TimeoutRescheduled>.All,
        Consumes<TimeoutExpired>.All,
        Consumes<EndpointIsHealthy>.All,
        Consumes<EndpointIsDown>.All,
        Consumes<EndpointIsSuspect>.All,
        Consumes<EndpointIsOffline>.All,
        Consumes<IWorkerAvailable>.All,
        IDisposable
    {

        private IServiceBus _bus;
        private readonly Guid _clientId = CombGuid.Generate();
        private StructureMap.IContainer _container;
        private IEndpoint _subscriptionServiceEndpoint;
        private UnsubscribeAction _unsubscribe;

        public SubscriptionDataConsumer()
        {
            BootstrapContainer();

            BootstrapServiceBus();

            ConnectToSubscriptionService();
        }

        private void ConnectToSubscriptionService()
        {
            _subscriptionServiceEndpoint = _container.GetInstance<IEndpointFactory>()
                .GetEndpoint(_container.GetInstance<IConfiguration>().SubscriptionServiceUri);

            _subscriptionServiceEndpoint.Send(new AddSubscriptionClient(_clientId, _bus.Endpoint.Uri, _bus.Endpoint.Uri));
        }

        private void BootstrapServiceBus()
        {
            MsmqEndpointConfigurator.Defaults(x =>
            {
                x.CreateMissingQueues = true;
            });

            _bus = _container.GetInstance<IServiceBus>();
            _unsubscribe = _bus.Subscribe(this);
        }

        private void BootstrapContainer()
        {
            _container = new StructureMap.Container();
            _container.Configure(x =>
            {
                x.ForRequestedType<IConfiguration>()
                    .CacheBy(InstanceScope.Singleton)
                    .AddConcreteType<Configuration>();
            });

            var registry = new SystemViewRegistry(_container);
            _container.Configure(x => x.AddRegistry(registry));
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

        public void Consume(HealthUpdate message)
        {
        }

        public void Consume(TimeoutScheduled message)
        {
        }

        public void Consume(TimeoutRescheduled message)
        {
        }

        public void Consume(TimeoutExpired message)
        {
        }

        public void Consume(EndpointIsHealthy message)
        {
        }

        public void Consume(EndpointIsDown message)
        {
        }

        public void Consume(EndpointIsSuspect message)
        {
        }

        public void Consume(EndpointIsOffline message)
        {
        }

        public void Consume(IWorkerAvailable message)
        {
        }

        public void Dispose()
        {
            _unsubscribe();
        }
    }
}