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
namespace MassTransit.Transports.RabbitMq.Tests
{
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class PublisherConfirm_Specs :
        Given_two_rabbitmq_buses_walk_into_a_bar
    {
        [Test]
        public void Should_call_the_ack_method_upon_delivery()
        {
            RemoteBus.Publish(new A
            {
                StringA = "ValueA",
            });

            _received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
        }

        Future<A> _received;

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);


            _received = new Future<A>();

            configurator.Subscribe(s => s.Handler<A>(message => _received.Complete(message)));
        }


        class A
        {
            public string StringA { get; set; }
        }


        class B
        {
            public string StringB { get; set; }
        }
    }


    [TestFixture]
    public class Publishing_without_waiting_for_an_ack :
        Given_two_rabbitmq_buses_walk_into_a_bar
    {
        [Test]
        public void Should_call_the_ack_method_upon_delivery()
        {
            RemoteBus.Publish(new A
            {
                StringA = "ValueA",
            }, x => x.SetWaitForAck(false));

            _received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
        }

        Future<A> _received;

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);


            _received = new Future<A>();

            configurator.Subscribe(s => s.Handler<A>(message => _received.Complete(message)));
        }


        class A
        {
            public string StringA { get; set; }
        }


        class B
        {
            public string StringB { get; set; }
        }
    }
}