// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using BusConfigurators;
    using Magnum.Extensions;
    using MassTransit.Subscriptions.Coordinator;
    using MassTransit.Transports.Loopback;
    using NUnit.Framework;


    [TestFixture]
    public class LoopbackLocalAndRemoteTestFixture :
        EndpointTestFixture<LoopbackTransportFactory>
    {
        public IServiceBus LocalBus { get; protected set; }
        public IServiceBus RemoteBus { get; protected set; }

        public LoopbackLocalAndRemoteTestFixture()
        {
            Instances = new Dictionary<string, ServiceInstance>();
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus = ServiceBusFactory.New(ConfigureLocalBus);

            RemoteBus = ServiceBusFactory.New(ConfigureRemoteBus);

            _localLoopback.SetTargetCoordinator(_remoteLoopback.Router);
            _remoteLoopback.SetTargetCoordinator(_localLoopback.Router);
        }

        protected Dictionary<string, ServiceInstance> Instances { get; private set; }

        protected ServiceInstance AddInstance(string instanceName, string queueName,
            Action<ServiceBusConfigurator> configureBus)
        {
            var instance = new ServiceInstance(queueName, x =>
                {
                    x.AddSubscriptionObserver((bus, coordinator) =>
                    {
                        var loopback = new SubscriptionLoopback(bus, coordinator);
                        loopback.SetTargetCoordinator(_localLoopback.Router);
                        return loopback;
                    });
                    x.AddSubscriptionObserver((bus, coordinator) =>
                    {
                        var loopback = new SubscriptionLoopback(bus, coordinator);
                        loopback.SetTargetCoordinator(_remoteLoopback.Router);
                        return loopback;
                    });

                    configureBus(x);
                });

            Instances.Add(instanceName, instance);

            return instance;
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

            foreach (ServiceInstance activityTestContext in Instances.Values)
                activityTestContext.ConfigureServiceBus(configurator);
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

            foreach (ServiceInstance activityTestContext in Instances.Values)
                activityTestContext.ConfigureServiceBus(configurator);
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

            Instances.Each(x => x.Value.Dispose());

            Instances.Clear();

            base.TeardownContext();
        }
    }
}