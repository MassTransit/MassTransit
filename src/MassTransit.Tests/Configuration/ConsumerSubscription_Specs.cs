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
namespace MassTransit.Tests.Configuration
{
    using System;
    using Magnum.Extensions;
    using NUnit.Framework;
    using Messages;
    using TestFramework;
    using TestFramework.Messages;


    
    public class When_subscribing_a_consumer_to_the_bus
    {
        IServiceBus _bus;
        PingMessage _ping;

        [SetUp]
        public void Subscribing_a_consumer_to_the_bus()
        {
            _bus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom("loopback://localhost/mt_test");

//                    x.Subscribe(s => s.Consumer<ConsumerOf<PingMessage>>());
                });

            _ping = new PingMessage(Guid.NewGuid());
            _bus.Publish(_ping);
        }

        [TearDown]
        public void Finally()
        {
            _bus.Dispose();
        }

        [Test]
        public void Should_have_subscribed()
        {
            _bus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
        }

        [Test]
        public void Should_have_received_the_message()
        {
            ConsumerOf<PingMessage>.AnyShouldHaveReceivedMessage(_ping, 8.Seconds());
        }
    }

    
    public class When_subscribing_a_type_of_consumer_to_the_bus
    {
        IServiceBus _bus;
        PingMessage _ping;

        [SetUp]
        public void Subscribing_a_consumer_to_the_bus()
        {
            _bus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom("loopback://localhost/mt_test");

//                    x.Subscribe(s => s.Consumer(typeof (ConsumerOf<PingMessage>), Activator.CreateInstance));
                });

            _ping = new PingMessage(Guid.NewGuid());
            _bus.Publish(_ping);
        }

        [TearDown]
        public void Finally()
        {
            _bus.Dispose();
        }

        [Test]
        public void Should_have_subscribed()
        {
            _bus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
        }

        [Test]
        public void Should_have_received_the_message()
        {
            ConsumerOf<PingMessage>.AnyShouldHaveReceivedMessage(_ping, 12.Seconds());
        }
    }

    
    public class When_subscribing_a_consumer_to_the_bus_with_a_factory_method
    {
        IServiceBus _bus;
        PingMessage _ping1;
        PingMessage _ping2;


        [SetUp]
        public void Subscribing_a_consumer_to_the_bus()
        {
            _bus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom("loopback://localhost/mt_test");
                    x.SetConcurrentConsumerLimit(1);

//                    x.Subscribe(s => s.Consumer(GetConsumer));
                });

            _ping1 = new PingMessage(Guid.NewGuid());
            _ping2 = new PingMessage(Guid.NewGuid());
            _bus.Publish(_ping1);
            _bus.Publish(_ping2);
        }

        [TearDown]
        public void Finally()
        {
            _bus.Dispose();
        }

        [Test]
        public void Should_have_subscribed()
        {
            _bus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
        }

        [Test]
        public void Should_have_received_the_message()
        {
            ConsumerOf<PingMessage>.AnyShouldHaveReceivedMessage(_ping1, 12.Seconds());
        }

        [Test]
        public void Should_have_received_the_second_message()
        {
            ConsumerOf<PingMessage>.AnyShouldHaveReceivedMessage(_ping2, 12.Seconds());
        }

        ConsumerOf<PingMessage> GetConsumer()
        {
            return new ConsumerOf<PingMessage>();
        }
    }
}