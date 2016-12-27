// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Testing
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;


    public class When_a_handler_responds_to_a_message
    {
        HandlerTestHarness<A> _handler;
        InMemoryTestHarness _harness;

        [OneTimeSetUp]
        public async Task A_handler_responds_to_a_message()
        {
            _harness = new InMemoryTestHarness();
            _handler = _harness.Handler<A>(async context => context.Respond(new B()));

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A(), x => x.ResponseAddress = _harness.BusAddress);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_have_sent_a_message_of_type_b()
        {
            _harness.Sent.Select<B>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_sent_message_to_bus_address()
        {
            var message = _harness.Sent.Select<B>().First();
            message.ShouldNotBeNull();

            message.Context.DestinationAddress.ShouldBe(_harness.BusAddress);
        }

        [Test]
        public void Should_support_a_simple_handler()
        {
            _handler.Consumed.Select().Any().ShouldBe(true);
        }


        class A
        {
        }


        class B
        {
        }
    }
}