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
namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EndpointConfigurators;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Sending_a_message_to_the_endpoint :
        InMemoryTestFixture
    {
        [Test]
        public void Should_start_the_handler_properly()
        {
        }

        [Test]
        public async void Should_be_received_by_the_handler()
        {
            await InputQueueSendEndpoint.Send(new A());

            await _receivedA;
        
            Assert.IsTrue(Sent.Select<A>().Any());

            var message = Sent.Select<A>().First();

            Assert.AreEqual(InputQueueAddress, message.Context.DestinationAddress);
        }

        Task<ConsumeContext<A>> _receivedA;


        class A
        {
        }


        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handler<A>(configurator, async (context) => Console.WriteLine("Hi"));
        }
    }


    [TestFixture]
    public class Sending_an_object_to_the_endpoint :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_be_received_by_the_handler()
        {
            object message = new A();

            await InputQueueSendEndpoint.Send(message);

            await _receivedA;

            Assert.IsTrue(Sent.Select<A>().Any());
        }

        Task<ConsumeContext<A>> _receivedA;


        class A
        {
        }


        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handled<A>(configurator);
        }
    }
}