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
namespace MassTransit.Transports.Msmq.Tests.TestFixtures
{
    using System;
    using BusConfigurators;
    using MassTransit.Tests.TextFixtures;
    using Subscriptions.Coordinator;

    public class MsmqEndpointTestFixture :
        EndpointTestFixture<MsmqTransportFactory>
    {
        protected Uri LocalEndpointUri { get; set; }
        protected Uri LocalErrorUri { get; set; }
        protected Uri RemoteEndpointUri { get; set; }
        protected Uri FaultEndpointUri { get; set; }

        protected IServiceBus LocalBus { get; set; }
        protected IServiceBus RemoteBus { get; set; }
        protected IServiceBus FaultBus { get; set; }

        public MsmqEndpointTestFixture()
            : this(new EndpointSettings(new MsmqEndpointAddress(new Uri("msmq://localhost/mt_client"))))
        {
        }

        public MsmqEndpointTestFixture(EndpointSettings settings)
        {
            LocalEndpointUri = settings.Address.Uri;
            LocalErrorUri = settings.ErrorAddress.Uri;
            RemoteEndpointUri = new Uri("msmq://localhost/mt_server");
            FaultEndpointUri = new Uri("msmq://localhost/mt_fault?tx=false");

            ConfigureEndpointFactory(x =>
                {
                    x.SetCreateMissingQueues(true);
                    x.SetCreateTransactionalQueues(settings.Transactional);
                    x.SetPurgeOnStartup(true);
                });
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalEndpoint = EndpointCache.GetEndpoint(LocalEndpointUri);
            LocalErrorEndpoint = EndpointCache.GetEndpoint(LocalErrorUri);
            RemoteEndpoint = EndpointCache.GetEndpoint(RemoteEndpointUri);
            FaultEndpoint = EndpointCache.GetEndpoint(RemoteEndpointUri);

            LocalBus = ServiceBusFactory.New(ConfigureLocalBus);

            RemoteBus = ServiceBusFactory.New(ConfigureRemoteBus);

            FaultBus = ServiceBusFactory.New(ConfigureFaultBus);

            _localLoopback.SetTargetCoordinator(_remoteLoopback.Router);
            _remoteLoopback.SetTargetCoordinator(_localLoopback.Router);
            _localFaultLoopback.SetTargetCoordinator(_localLoopback.Router);
            _remoteFaultLoopback.SetTargetCoordinator(_remoteLoopback.Router);
        }

        SubscriptionLoopback _localLoopback;
        SubscriptionLoopback _remoteLoopback;
        SubscriptionLoopback _localFaultLoopback;
        SubscriptionLoopback _remoteFaultLoopback;

        protected virtual void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            configurator.ReceiveFrom(LocalEndpointUri);
            configurator.AddSubscriptionObserver((bus, coordinator) =>
                {
                    _localLoopback = new SubscriptionLoopback(bus, coordinator);
                    return _localLoopback;
                });
        }

        protected virtual void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            configurator.ReceiveFrom(RemoteEndpointUri);
            configurator.AddSubscriptionObserver((bus, coordinator) =>
                {
                    _remoteLoopback = new SubscriptionLoopback(bus, coordinator);
                    return _remoteLoopback;
                });
        }

        protected virtual void ConfigureFaultBus(ServiceBusConfigurator configurator)
        {
            configurator.SetConcurrentConsumerLimit(1);
            configurator.ReceiveFrom(FaultEndpointUri);
            configurator.AddSubscriptionObserver((bus, coordinator) =>
                {
                    _localFaultLoopback = new SubscriptionLoopback(bus, coordinator);
                    return _localFaultLoopback;
                });
            configurator.AddSubscriptionObserver((bus, coordinator) =>
                {
                    _remoteFaultLoopback = new SubscriptionLoopback(bus, coordinator);
                    return _remoteFaultLoopback;
                });
        }

        protected IEndpoint LocalEndpoint { get; set; }
        protected IEndpoint LocalErrorEndpoint { get; set; }
        protected IEndpoint RemoteEndpoint { get; set; }
        protected IEndpoint FaultEndpoint { get; set; }

        protected override void TeardownContext()
        {
            LocalBus.Dispose();
            LocalBus = null;

            RemoteBus.Dispose();
            RemoteBus = null;

            FaultBus.Dispose();
            FaultBus = null;

            LocalEndpoint = null;
            LocalErrorEndpoint = null;
            RemoteEndpoint = null;
            FaultEndpoint = null;

            base.TeardownContext();
        }
    }
}