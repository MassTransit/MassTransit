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
    using System.Threading.Tasks;
    using Magnum.Extensions;
    using Messages;
    using NUnit.Framework;
    using TestConsumers;
    using TestFramework;
    using TextFixtures;


    [TestFixture]
    public class PublishSubscribe_Specs :
        LoopbackTestFixture
    {
        [Test]
        public void A_simple_bus_should_be_able_to_subscribe_and_publish()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            LocalBus.SubscribeInstance(consumer);

            var message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, 8.Seconds());
        }
    }


    [TestFixture]
    public class Sending_a_message_through_the_bus_pipeline :
        LoopbackTestFixture
    {
        [Test]
        public void A_simple_bus_should_be_able_to_subscribe_and_publish()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            LocalBus.SubscribeInstance(consumer);

            var message = new PingMessage();
            LocalBus.InboundPipe.Send(new TestConsumeContext<PingMessage>(message));

            consumer.ShouldHaveReceivedMessage(message, 8.Seconds());
        }
    }


    [TestFixture]
    public class Publishing_an_object_to_the_bus :
        LoopbackTestFixture
    {
        [Test]
        public void Should_accept_the_type_specified()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            LocalBus.SubscribeInstance(consumer);

            var message = new PingMessage();

            object unknownMessage = message;
            LocalBus.Publish(unknownMessage, typeof(PingMessage));

            consumer.ShouldHaveReceivedMessage(message, 8.Seconds());
        }

        [Test]
        public void Should_accept_the_type_specified_with_context()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            LocalBus.SubscribeInstance(consumer);

            var message = new PingMessage();

            object unknownMessage = message;
            LocalBus.Publish(unknownMessage, typeof(PingMessage), x => x.SetRequestId("27"));

            consumer.ShouldHaveReceivedMessage(message, 8.Seconds());
        }

        [Test]
        public void Should_receive_the_proper_message()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            LocalBus.SubscribeInstance(consumer);

            var message = new PingMessage();

            object unknownMessage = message;
            LocalBus.Publish(unknownMessage);

            consumer.ShouldHaveReceivedMessage(message, 8.Seconds());
        }
    }


    [TestFixture]
    public class Sending_an_object_to_the_bus :
        LoopbackTestFixture
    {
        [Test]
        public void Should_accept_the_type_specified()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            LocalBus.SubscribeInstance(consumer);

            var message = new PingMessage();

            object unknownMessage = message;
            LocalBus.Endpoint.Send(unknownMessage, typeof(PingMessage));

            consumer.ShouldHaveReceivedMessage(message, 8.Seconds());
        }

        [Test]
        public void Should_accept_the_type_specified_with_context()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            LocalBus.SubscribeInstance(consumer);

            var message = new PingMessage();

            object unknownMessage = message;
            LocalBus.Endpoint.Send(unknownMessage, typeof(PingMessage), x => x.SetRequestId("27"));

            consumer.ShouldHaveReceivedMessage(message, 8.Seconds());
        }

        [Test]
        public void Should_receive_the_proper_message()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            LocalBus.SubscribeInstance(consumer);

            var message = new PingMessage();

            object unknownMessage = message;
            LocalBus.Endpoint.Send(unknownMessage);

            consumer.ShouldHaveReceivedMessage(message, 8.Seconds());
        }
    }


    [TestFixture]
    public class Sending_an_object_to_the_local_bus :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_receive_the_proper_message()
        {
            Task<A> handler = SubscribeToLocalBus<A>();

            ISendEndpoint sendEndpoint = await LocalBusSendEndpoint;

            object message = new A();
            await sendEndpoint.Send(message);

            await handler;
        }

        [Test]
        public async void Should_receive_the_proper_message_as_a()
        {
            Task<A> handler = SubscribeToLocalBus<A>();

            ISendEndpoint sendEndpoint = await LocalBusSendEndpoint;

            var message = new A();
            await sendEndpoint.Send(message);

            await handler;
        }

        [Test]
        public async void Should_receive_the_proper_message_as_a_with_request_id()
        {
            Task<A> handler = SubscribeToLocalBus<A>(x => x.RequestId.HasValue);

            ISendEndpoint sendEndpoint = await LocalBusSendEndpoint;

            var message = new A();
            await sendEndpoint.Send(message, Pipe.New<SendContext<A>>(x => x.Execute(c => c.RequestId = NewId.NextGuid())));

            await handler;
        }


        class A
        {
        }
    }
}