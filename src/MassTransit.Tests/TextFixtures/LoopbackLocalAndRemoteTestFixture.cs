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
    using MassTransit.Transports.Loopback;
    using NUnit.Framework;


    [TestFixture, Obsolete]
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
        }

        protected Dictionary<string, ServiceInstance> Instances { get; private set; }

        protected ServiceInstance AddInstance(string instanceName, string queueName,
            Action<ServiceBusConfigurator> configureBus)
        {
            var instance = new ServiceInstance(queueName, x =>
                {
                    configureBus(x);
                });

            Instances.Add(instanceName, instance);

            return instance;
        }


        protected Uri LocalUri;
        protected Uri RemoteUri;

        protected virtual void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            LocalUri = new Uri("loopback://localhost/mt_client");
            configurator.ReceiveFrom(LocalUri);

            foreach (ServiceInstance activityTestContext in Instances.Values)
                activityTestContext.ConfigureServiceBus(configurator);
        }

        protected virtual void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            RemoteUri = new Uri("loopback://localhost/mt_server");
            configurator.ReceiveFrom(RemoteUri);

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