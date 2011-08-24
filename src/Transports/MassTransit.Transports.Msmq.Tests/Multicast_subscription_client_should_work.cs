// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Diagnostics;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Tests;
    using MassTransit.Tests.Messages;
    using NUnit.Framework;
    using TestFixtures;
    using TestFramework;

    [TestFixture, Integration]
    public class Multicast_subscription_client_should_work :
        MulticastMsmqEndpointTestFixture
    {
        private PingMessage _ping;
        private FutureMessage<PingMessage, Guid> _future;
        UnsubscribeAction _unsubscribe;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _ping = new PingMessage();
            _future = new FutureMessage<PingMessage, Guid>(_ping.CorrelationId);

            _unsubscribe = RemoteBus.SubscribeHandler<PingMessage>(message => { _future.Set(message); });

            LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();

            Trace.WriteLine("LocalBus");

            LocalBus.OutboundPipeline.Trace();

            Trace.WriteLine("RemoteBus");

            RemoteBus.OutboundPipeline.Trace();

            LocalBus.Publish(_ping);
        }

        protected override void TeardownContext()
        {
            _unsubscribe();

            LocalBus.ShouldNotHaveSubscriptionFor<PingMessage>();

            base.TeardownContext();
        }

        protected override void ConfigureLocalBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.SetNetwork("ONE");
        }

        protected override void ConfigureRemoteBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);
            
            configurator.SetNetwork("ONE");
        }

        [Test]
        public void The_message_should_arrive()
        {
            _future.WaitUntilAvailable(10.Seconds());
        }
    }

    [TestFixture, Integration]
    public class Multicast_subscription_client_with_control_bus_should_work :
        MulticastMsmqEndpointTestFixture
    {
        private PingMessage _ping;
        private FutureMessage<PingMessage, Guid> _future;
        UnsubscribeAction _unsubscribe;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _ping = new PingMessage();
            _future = new FutureMessage<PingMessage, Guid>(_ping.CorrelationId);

            _unsubscribe = RemoteBus.SubscribeHandler<PingMessage>(message => { _future.Set(message); });


            RemoteBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
    
            LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();

            Trace.WriteLine("LocalBus");

            LocalBus.OutboundPipeline.Trace();

            Trace.WriteLine("RemoteBus");

            RemoteBus.OutboundPipeline.Trace();


            LocalBus.Publish(_ping);
        }

        protected override void ConfigureLocalBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.UseControlBus();
        }

        protected override void ConfigureRemoteBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.UseControlBus();
        }

        protected override void TeardownContext()
        {
            _unsubscribe();

            LocalBus.ShouldNotHaveSubscriptionFor<PingMessage>();

            base.TeardownContext();
        }

        [Test]
        public void The_message_should_arrive()
        {
            _future.WaitUntilAvailable(10.Seconds());
        }
    }
}