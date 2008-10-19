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
namespace MassTransit.Infrastructure.Tests
{
    using System;
    using System.Threading;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using ServiceBus;
    using ServiceBus.Tests;

    [TestFixture]
    public class When_multiple_service_bus_instances_share_a_distributed_subscription_cache :
        LocalAndRemoteTestContext
    {
        [Test]
        public void Correlated_messages_should_be_delivered()
        {
            Guid correlationId = Guid.NewGuid();
            CorrelatedConsumer consumer = new CorrelatedConsumer(correlationId);

            RemoteBus.Subscribe(consumer);

            var dm = new CorrelatedMessage(correlationId);

            LocalBus.Publish(dm);

            Assert.That(consumer.Received.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
                        "Timeout expired waiting for message");

        }

        [Test]
        public void Correlated_messages_should_be_delivered_to_both_correlated_and_regular_consumers()
        {

            ManualResetEvent _received = new ManualResetEvent(false);

            RemoteBus.Subscribe<CorrelatedMessage>(
                delegate { _received.Set(); });

            Guid correlationId = Guid.NewGuid();

            CorrelatedConsumer consumer = new CorrelatedConsumer(correlationId);

            RemoteBus.Subscribe(consumer);

            CorrelatedMessage dm = new CorrelatedMessage(correlationId);

            LocalBus.Publish(dm);

            Assert.That(consumer.Received.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
                        "Timeout expired waiting for message");

            Assert.That(_received.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
                        "Timeout expired waiting for message");
			
        }

        [Test]
        public void Regular_messages_should_be_delivered()
        {
			
            ManualResetEvent _received = new ManualResetEvent(false);

            RemoteBus.Subscribe<PublishedMessage>(
                delegate { _received.Set(); });

            PublishedMessage dm = new PublishedMessage();

            LocalBus.Publish(dm);

            Assert.That(_received.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
                        "Timeout expired waiting for message");
			
        }

        [Serializable]
        internal class PublishedMessage
        {
        }

        [Serializable]
        internal class CorrelatedMessage : CorrelatedBy<Guid>
        {
            private readonly Guid _correlationId;

            public CorrelatedMessage(Guid correlationId)
            {
                _correlationId = correlationId;
            }

            public Guid CorrelationId
            {
                get { return _correlationId; }
            }
        }

        internal class CorrelatedConsumer : Consumes<CorrelatedMessage>.For<Guid>
        {
            private readonly Guid _correlationId;
            private readonly ManualResetEvent _received = new ManualResetEvent(false);

            public CorrelatedConsumer(Guid correlationId)
            {
                _correlationId = correlationId;
            }

            public ManualResetEvent Received
            {
                get { return _received; }
            }

            public void Consume(CorrelatedMessage message)
            {
                _received.Set();
            }

            public Guid CorrelationId
            {
                get { return _correlationId; }
            }
        }
    }
}