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
    using Magnum.TestFramework;
    using MassTransit.Subscriptions;
#if NET40
    using MassTransit.Testing;
    using Messages;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture]
    public class MultiConsumer_Specs :
        LoopbackTestFixture
    {
        class PingPongConsumer :
            MultiConsumer
        {
            public PingPongConsumer()
            {
                Consume<PingMessage>();
                Consume<PongMessage>();
            }
        }

        [Test]
        public void Should_show_that_the_message_was_received_by_the_consumer()
        {
            var multiConsumer = new MultiConsumer();
            ReceivedMessageList<PingMessage> received = multiConsumer.Consume<PingMessage>();

            multiConsumer.Subscribe(LocalBus);

            LocalBus.Publish(new PingMessage());

            received.Any().ShouldBeTrue();
        }

        [Test]
        public void Should_show_that_the_specified_type_was_received()
        {
            var consumer = new PingPongConsumer();

            using (IUnsubscribeAction unsubscribe = consumer.Subscribe(LocalBus).Disposable())
            {
                var pingMessage = new PingMessage();
                LocalBus.Publish(pingMessage);
                LocalBus.Publish(new PongMessage(pingMessage.CorrelationId));

                consumer.Received.Any<PingMessage>().ShouldBeTrue();
                consumer.Received.Any<PongMessage>(
                    (received, message) => message.CorrelationId == pingMessage.CorrelationId).ShouldBeTrue();
            }
        }

        [Test]
        public void Should_distinguish_multiple_events()
        {
            var consumer = new PingPongConsumer();

            using (IUnsubscribeAction unsubscribe = consumer.Subscribe(LocalBus).Disposable())
            {
                var pingMessage = new PingMessage();
                var pingMessage2 = new PingMessage();
                LocalBus.Publish(pingMessage);
                LocalBus.Publish(pingMessage2);

                consumer.Received.Any<PingMessage>(
                    (received, message) => message.CorrelationId == pingMessage.CorrelationId).ShouldBeTrue();
                consumer.Received.Any<PingMessage>(
                    (received, message) => message.CorrelationId == pingMessage2.CorrelationId).ShouldBeTrue();
            }
        }
    }
#endif
}