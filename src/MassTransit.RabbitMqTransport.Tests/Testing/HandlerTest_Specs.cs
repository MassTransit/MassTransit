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
namespace MassTransit.RabbitMqTransport.Tests.Testing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using MassTransit.Testing;
    using RabbitMqTransport.Testing;
    using Shouldly;


    [TestFixture]
    public class Using_the_handler_test_factory
    {
        RabbitMqTestHarness _harness;
        HandlerTestHarness<A> _handler;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _harness = new RabbitMqTestHarness();
            _handler = _harness.Handler<A>(async context =>
            {
                var endpoint = await context.GetSendEndpoint(context.SourceAddress);

                await endpoint.Send(new C());

                await context.Publish(new D());
            });

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
            await _harness.Bus.Publish(new B());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_have_received_a_message_of_type_a()
        {
            _harness.Consumed.Select<A>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_sent_a_message_of_type_a()
        {
            _harness.Sent.Select<A>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_published_a_message_of_type_b()
        {
            _harness.Published.Select<B>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_sent_a_message_of_type_c()
        {
            _harness.Sent.Select<C>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_published_a_message_of_type_d()
        {
            _harness.Published.Select<D>().Any().ShouldBe(true);
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


        class C
        {
        }


        class D
        {
        }
    }
}