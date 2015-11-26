// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Sending_a_message_to_the_endpoint :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_by_the_handler()
        {
            await InputQueueSendEndpoint.Send(new A());

            await _receivedA;
        }

        [Test]
        public void Should_start_the_handler_properly()
        {
        }

        Task<ConsumeContext<A>> _receivedA;


        class A
        {
        }


        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handler<A>(configurator, async context => Console.WriteLine("Hi"));
        }
    }


    [TestFixture]
    public class Sending_an_object_to_the_endpoint :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_by_the_handler()
        {
            object message = new A();

            await InputQueueSendEndpoint.Send(message);

            await _receivedA;
        }

        Task<ConsumeContext<A>> _receivedA;


        class A
        {
        }


        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handled<A>(configurator);
        }
    }

    [TestFixture]
    public class Publishing_a_message_on_the_bus_using_the_InMemoryTransport
        : AsyncTestFixture
    {
        class TestMessage
        { }

        [Test, Explicit]
        public async Task Throw_an_an_uncatchable_ArgumentException()
        {
            var busControl = MassTransit.Bus.Factory.CreateUsingInMemory(x => { });

            await busControl.StartAsync();

            await busControl.Publish(new TestMessage());

            await Task.Delay(5000);

            await busControl.StopAsync();

        }

        [Test, Explicit]
        public async Task Should_not_move_the_message_to_the_dead_letter_queue()
        {

            var busControl = MassTransit.Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ReceiveEndpoint("recv_queue", q => q.Handler<TestMessage>(async m => { }));
            });

            await busControl.StartAsync();

            await busControl.Publish(new TestMessage());

            await Task.Delay(5000);

            await busControl.StopAsync();

        }
    }

}