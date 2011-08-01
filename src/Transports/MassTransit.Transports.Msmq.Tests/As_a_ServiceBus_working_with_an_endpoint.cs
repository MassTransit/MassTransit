// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Tests;
    using MassTransit.Tests.Messages;
    using MassTransit.Tests.TestConsumers;
    using NUnit.Framework;
    using TestFixtures;
    using TestFramework;
    using MassTransit.Testing;

	[TestFixture, Integration]
    public class When_a_message_is_published_to_a_transactional_queue :
        MsmqTransactionalEndpointTestFixture
    {
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        [Test]
        public void It_should_be_received_by_one_subscribed_consumer()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.SubscribeInstance(consumer);

        	LocalBus.ShouldHaveSubscriptionFor<PingMessage>();
            var message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, _timeout);
        }

        [Test]
        public void It_should_leave_the_message_in_the_queue_if_an_exception_is_thrown()
        {
			FutureMessage<PingMessage> future = new FutureMessage<PingMessage>();

            RemoteBus.SubscribeHandler<PingMessage>(m =>
            	{
            		future.Set(m);

            		throw new ApplicationException("Boing!");
            	});

			LocalBus.ShouldHaveSubscriptionFor<PingMessage>();
			var message = new PingMessage();
            LocalBus.Publish(message);

        	future.IsAvailable(_timeout).ShouldBeTrue("Message was not received");

        	RemoteEndpoint.ShouldNotContain(message);
        }

        [Test]
        public void It_should_not_rollback_a_send_if_an_exception_is_thrown()
        {
            var consumer = new TestMessageConsumer<PongMessage>();
            LocalBus.SubscribeInstance(consumer);

            var message = new PingMessage();
            var response = new PongMessage(message.CorrelationId);

            RemoteBus.SubscribeHandler<PingMessage>(m =>
            {
                RemoteBus.Publish(response);
                throw new ApplicationException("Boing!");
            });

			LocalBus.ShouldHaveSubscriptionFor<PingMessage>();
			RemoteBus.ShouldHaveSubscriptionFor<PongMessage>();
			LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(response, _timeout);
        }
    }

    [TestFixture, Integration]
    public class When_publishing_a_message :
        MsmqEndpointTestFixture
    {

        [Test]
        public void Multiple_Local_Services_Should_Be_Available()
        {
        	var updated = new Future<UpdateMessage>();
        	var deleted = new Future<DeleteMessage>();

        	LocalBus.SubscribeHandler<UpdateMessage>(updated.Complete);
        	LocalBus.SubscribeHandler<DeleteMessage>(deleted.Complete);

            var dm = new DeleteMessage();
            LocalBus.Publish(dm);

            var um = new UpdateMessage();
            LocalBus.Publish(um);

        	updated.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Update not received");
        	deleted.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Delete not received");
        }

        [Test]
        public void Multiple_messages_should_be_delivered_to_the_appropriate_remote_subscribers()
        {
			var updated = new Future<UpdateMessage>();
			var deleted = new Future<DeleteMessage>();

			LocalBus.SubscribeHandler<UpdateMessage>(updated.Complete);
			LocalBus.SubscribeHandler<DeleteMessage>(deleted.Complete);

        	RemoteBus.HasSubscription<UpdateMessage>().Count().ShouldBeGreaterThan(0);
			RemoteBus.HasSubscription<DeleteMessage>().Count().ShouldBeGreaterThan(0);

			var dm = new DeleteMessage();
			RemoteBus.Publish(dm);

			var um = new UpdateMessage();
			RemoteBus.Publish(um);

			updated.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Update not received");
			deleted.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Delete not received");
        }

        [Test]
        public void The_message_should_be_delivered_to_a_local_subscriber()
        {
			var updated = new Future<UpdateMessage>();

			LocalBus.SubscribeHandler<UpdateMessage>(updated.Complete);

			var um = new UpdateMessage();
			LocalBus.Publish(um);

			updated.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Update not received");
        }

        [Test]
        public void The_message_should_be_delivered_to_a_remote_subscriber()
        {
			var updated = new Future<UpdateMessage>();
		
			LocalBus.SubscribeHandler<UpdateMessage>(updated.Complete);

			RemoteBus.HasSubscription<UpdateMessage>().Count().ShouldBeGreaterThan(0);

			var um = new UpdateMessage();
			RemoteBus.Publish(um);

			updated.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Update not received");
		}

        [Test]
        public void The_message_should_be_delivered_to_a_remote_subscriber_with_a_reply()
        {
			var updated = new Future<UpdateMessage>();
			var updateAccepted = new Future<UpdateAcceptedMessage>();

			RemoteBus.SubscribeContextHandler<UpdateMessage>(context =>
				{
					updated.Complete(context.Message);

					context.Respond(new UpdateAcceptedMessage());
				});

			LocalBus.HasSubscription<UpdateMessage>().Count().ShouldBeGreaterThan(0);

        	LocalBus.SubscribeHandler<UpdateAcceptedMessage>(updateAccepted.Complete);
        	RemoteBus.HasSubscription<UpdateAcceptedMessage>().Count().ShouldBeGreaterThan(0);

			var um = new UpdateMessage();
			LocalBus.Publish(um);

			updated.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Update not received");
			updateAccepted.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Update accepted not received");
	        }
    }

    [TestFixture, Integration]
    public class When_an_accept_method_throws_an_exception :
        MsmqEndpointTestFixture
    {
        [Test]
        public void The_exception_should_not_disrupt_the_flow_of_messages()
        {
            CrashingService service = new CrashingService();

            LocalBus.SubscribeInstance(service);

            LocalEndpoint.Send(new BogusMessage());

            CrashingService.Received.WaitOne(5.Seconds(), true).ShouldBeTrue("No message received");

            CrashingService.Received.Reset();

			LocalEndpoint.Send(new LegitMessage());

            CrashingService.LegitReceived.WaitOne(5.Seconds(), true).ShouldBeTrue("No message received");
        }

        internal class CrashingService :
            Consumes<BogusMessage>.All,
            Consumes<LegitMessage>.All
        {
            public static ManualResetEvent Received
            {
                get { return _received; }
            }

            private static readonly ManualResetEvent _received = new ManualResetEvent(false);

            public static ManualResetEvent LegitReceived
            {
                get { return _legitReceived; }
            }

            private static readonly ManualResetEvent _legitReceived = new ManualResetEvent(false);

            public void Consume(BogusMessage message)
            {
                _received.Set();

                throw new ApplicationException("Consumer goes boom!");
            }

            public void Consume(LegitMessage message)
            {
                _legitReceived.Set();
            }
        }

        [Serializable]
        internal class BogusMessage
        {
        }

        [Serializable]
        internal class LegitMessage
        {
        }
    }

    [TestFixture, Integration]
    public class When_receiving_messages_slowly :
        MsmqEndpointTestFixture
    {
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        [Test]
        public void It_should_be_received_by_one_subscribed_consumer()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.SubscribeInstance(consumer);

            Thread.Sleep(5.Seconds());

            var message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, _timeout);
        }
    }
}