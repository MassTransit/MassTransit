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
namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;
    using Tests.Messages;
    using Tests.TestConsumers;

    [TestFixture]
    public class When_a_request_message_is_published :
        LocalAndRemoteTestContext
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);

        [Test]
        public void A_reply_should_be_received_by_the_requestor()
        {
            // Arrange
            Container.AddComponent<TestReplyService<PingMessage, Guid, PongMessage>>();
            RemoteBus.Subscribe<TestReplyService<PingMessage, Guid, PongMessage>>();

            PingMessage message = new PingMessage();

            TestCorrelatedConsumer<PongMessage, Guid> consumer = new TestCorrelatedConsumer<PongMessage, Guid>(message.CorrelationId);
            LocalBus.Subscribe(consumer);

            // Act
            LocalBus.Publish(message);

            // Assert
            TestConsumerBase<PingMessage>.AnyShouldHaveReceivedMessage(message, _timeout);

            consumer.ShouldHaveReceivedMessage(new PongMessage(message.CorrelationId), _timeout);
        }
    }
}