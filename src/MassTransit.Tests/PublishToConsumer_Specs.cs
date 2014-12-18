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
    using NUnit.Framework;
    using TestConsumers;
    using TestFramework;
    using TestFramework.Messages;
    using TextFixtures;


    [TestFixture]
    public class When_a_message_is_published :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void It_should_be_ignored_if_there_are_no_consumers()
        {
            var message = new PingMessage();

            LocalBus.Publish(message);
        }

        [Test]
        public void It_should_be_received_by_a_component()
        {
            RemoteBus.ConnectConsumer<TestMessageConsumer<PingMessage>>();

            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var message = new PingMessage();
            LocalBus.Publish(message);

            TestConsumerBase<PingMessage>.AnyShouldHaveReceivedMessage(message, _timeout);
        }

        [Test]
        public void It_should_be_received_by_an_interested_correlated_consumer()
        {
            var message = new PingMessage();

            var consumer = new TestCorrelatedConsumer<PingMessage, Guid>(message.CorrelationId);
            RemoteBus.ConnectInstance(consumer);
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, _timeout);
        }

        [Test]
        public void It_should_be_received_by_multiple_subscribed_consumers()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.ConnectInstance(consumer);

            var messageConsumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.ConnectHandler<PingMessage>(messageConsumer.MessageHandler);

            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, _timeout);
            messageConsumer.ShouldHaveReceivedMessage(message, _timeout);
        }

        [Test]
        public void It_should_be_received_by_one_subscribed_consumer()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.ConnectInstance(consumer);

            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, _timeout);
        }

        [Test]
        public void It_should_be_received_by_one_subscribed_message_handler()
        {
            var messageConsumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.ConnectHandler<PingMessage>(messageConsumer.MessageHandler);

            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var message = new PingMessage();
            LocalBus.Publish(message);

            messageConsumer.ShouldHaveReceivedMessage(message, _timeout);
        }

        [Test]
        public void It_should_not_be_received_by_an_uninterested_correlated_consumer()
        {
            var consumer = new TestCorrelatedConsumer<PingMessage, Guid>(Guid.NewGuid());
            RemoteBus.ConnectInstance(consumer);

            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldNotHaveReceivedMessage(message, _timeout);
        }

        static readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);
    }
}