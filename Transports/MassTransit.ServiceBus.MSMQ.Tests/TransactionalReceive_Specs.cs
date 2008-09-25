// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.MSMQ.Tests

{
    using System;
    using MassTransit.ServiceBus.Tests;
    using MassTransit.ServiceBus.Tests.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class When_a_message_is_published_to_a_transactional_queue :
        LocalAndRemoteTestContext
    {
        protected override string GetCastleConfigurationFile()
        {
            return "transactional.castle.xml";
        }

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        [Test]
        public void It_should_be_received_by_one_subscribed_consumer()
        {
            TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.Subscribe(consumer);

            PingMessage message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, _timeout);
        }

        [Test]
        public void It_should_leave_the_message_in_the_queue_if_an_exception_is_thrown()
        {
            RemoteBus.Subscribe<PingMessage>(m => { throw new ApplicationException("Boing!"); });

            PingMessage message = new PingMessage();
            LocalBus.Publish(message);
        }

        [Test]
        public void It_should_rollback_a_send_if_an_exception_is_thrown()
        {
            TestMessageConsumer<PongMessage> consumer = new TestMessageConsumer<PongMessage>();
            LocalBus.Subscribe(consumer);

            PingMessage message = new PingMessage();
            PongMessage response = new PongMessage(message.CorrelationId);

            RemoteBus.Subscribe<PingMessage>(m =>
                {
                    RemoteBus.Publish(response);
                    throw new ApplicationException("Boing!");
                });

            LocalBus.Publish(message);

            consumer.ShouldNotHaveReceivedMessage(response, _timeout);
        }
    }
}