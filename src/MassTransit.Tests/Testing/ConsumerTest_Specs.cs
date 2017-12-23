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
namespace MassTransit.Tests.Testing
{
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using MassTransit.Testing;
    using Shouldly;

    [TestFixture]
    public class When_a_consumer_is_being_tested
    {
        InMemoryTestHarness _harness;
        ConsumerTestHarness<Testsumer> _consumer;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _consumer = _harness.Consumer<Testsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }


        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            _harness.Sent.Select<A>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_sent_the_response_from_the_consumer()
        {
            _harness.Published.Select<B>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            _harness.Consumed.Select<A>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_called_the_consumer_method()
        {
            _consumer.Consumed.Select<A>().Any().ShouldBe(true);
        }

        class Testsumer :
            IConsumer<A>
        {
            public async Task Consume(ConsumeContext<A> context)
            {
                await context.RespondAsync(new B());
            }
        }

        class A
        {
        }

        class B
        {
        }
    }

    public class When_a_context_consumer_is_being_tested
    {
        InMemoryTestHarness _harness;
        ConsumerTestHarness<Testsumer> _consumer;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _consumer = _harness.Consumer<Testsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A(), context => context.ResponseAddress = _harness.BusAddress);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            _harness.Sent.Select<A>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_sent_the_response_from_the_consumer()
        {
            _harness.Sent.Select<B>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            _harness.Consumed.Select<A>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_called_the_consumer_method()
        {
            _consumer.Consumed.Select<A>().Any().ShouldBe(true);
        }

        class Testsumer :
            IConsumer<A>
        {
            public Task Consume(ConsumeContext<A> context)
            {
                return context.RespondAsync(new B());
            }
        }

        class A
        {
        }

        class B
        {
        }
    }
}