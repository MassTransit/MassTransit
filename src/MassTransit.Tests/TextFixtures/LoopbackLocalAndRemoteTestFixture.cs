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
namespace MassTransit.Tests.TextFixtures
{
    using System;
    using BusConfigurators;
    using MassTransit.Subscriptions.Coordinator;
    using MassTransit.Transports.Loopback;
    using NUnit.Framework;

    [TestFixture]
    public class LoopbackLocalAndRemoteTestFixture :
        EndpointTestFixture<LoopbackTransportFactory>
    {
        public IServiceBus LocalBus { get; protected set; }
        public IServiceBus RemoteBus { get; protected set; }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus = ServiceBusFactory.New(ConfigureLocalBus);

            RemoteBus = ServiceBusFactory.New(ConfigureRemoteBus);

            _localLoopback.SetTargetCoordinator(_remoteLoopback.Router);
            _remoteLoopback.SetTargetCoordinator(_localLoopback.Router);
        }

        SubscriptionLoopback _localLoopback;
        SubscriptionLoopback _remoteLoopback;
        protected Uri LocalUri;
        protected Uri RemoteUri;

        protected virtual void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            LocalUri = new Uri("loopback://localhost/mt_client");
            configurator.ReceiveFrom(LocalUri);
            configurator.AddSubscriptionObserver((bus, coordinator) =>
                {
                    _localLoopback = new SubscriptionLoopback(bus, coordinator);
                    return _localLoopback;
                });
        }

        protected virtual void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            RemoteUri = new Uri("loopback://localhost/mt_server");
            configurator.ReceiveFrom(RemoteUri);
            configurator.AddSubscriptionObserver((bus, coordinator) =>
                {
                    _remoteLoopback = new SubscriptionLoopback(bus, coordinator);
                    return _remoteLoopback;
                });
        }

        protected override void TeardownContext()
        {
            if (RemoteBus != null)
            {
                RemoteBus.Dispose();
                RemoteBus = null;
            }

            if (LocalBus != null)
            {
                LocalBus.Dispose();
                LocalBus = null;
            }

            base.TeardownContext();
        }
    }
}