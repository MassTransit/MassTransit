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
namespace MassTransit.Tests
{
	using System;
	using System.Collections.Generic;
	using Magnum.Extensions;
	using Messages;
	using NUnit.Framework;
	using TestConsumers;
	using TestFramework;
	using TextFixtures;

	[TestFixture]
	public class MessageContext_Specs :
		LoopbackLocalAndRemoteTestFixture
	{
		[Test]
		public void A_response_should_be_published_if_no_reply_address_is_specified()
		{
			var ping = new PingMessage();

			var otherConsumer = new TestMessageConsumer<PongMessage>();
			RemoteBus.SubscribeInstance(otherConsumer);

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(ping.CorrelationId);
			LocalBus.SubscribeInstance(consumer);

			var pong = new FutureMessage<PongMessage>();

			RemoteBus.SubscribeHandler<PingMessage>(message =>
				{
					pong.Set(new PongMessage(message.CorrelationId));

					RemoteBus.Context().Respond(pong.Message);
				});

			LocalBus.Publish(ping);

			Assert.IsTrue(pong.IsAvailable(3.Seconds()), "No pong generated");

			consumer.ShouldHaveReceivedMessage(pong.Message, 3.Seconds());
			otherConsumer.ShouldHaveReceivedMessage(pong.Message, 1.Seconds());
		}

		[Test]
		public void A_response_should_be_sent_directly_if_a_reply_address_is_specified()
		{
			var ping = new PingMessage();

			var otherConsumer = new TestMessageConsumer<PongMessage>();
			RemoteBus.SubscribeInstance(otherConsumer);

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(ping.CorrelationId);
			LocalBus.SubscribeInstance(consumer);

			var pong = new FutureMessage<PongMessage>();

			RemoteBus.SubscribeHandler<PingMessage>(message =>
				{
					pong.Set(new PongMessage(message.CorrelationId));

					RemoteBus.Context().Respond(pong.Message);
				});

			RemoteBus.ShouldHaveRemoteSubscriptionFor<PongMessage>();
			LocalBus.ShouldHaveRemoteSubscriptionFor<PongMessage>();
			LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();

			LocalBus.Publish(ping, context => context.SendResponseTo(LocalBus));

			Assert.IsTrue(pong.IsAvailable(8.Seconds()), "No pong generated");

			consumer.ShouldHaveReceivedMessage(pong.Message, 8.Seconds());
			otherConsumer.ShouldNotHaveReceivedMessage(pong.Message, 1.Seconds());
		}

		[Test]
		public void The_destination_address_should_pass()
		{
			var received = new FutureMessage<PingMessage>();

			LocalBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Address.Uri, LocalBus.Context().DestinationAddress);

					received.Set(message);
				});

			LocalBus.Publish(new PingMessage());

			Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
		}

		[Test]
		public void The_fault_address_should_pass()
		{
			var received = new FutureMessage<PingMessage>();

			LocalBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Address.Uri, LocalBus.Context().FaultAddress);

					received.Set(message);
				});

			LocalBus.Publish(new PingMessage(), context => context.SendFaultTo(LocalBus));

			Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
		}

		[Test]
		public void The_response_address_should_pass()
		{
			var received = new FutureMessage<PingMessage>();

			LocalBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Address.Uri, LocalBus.Context().ResponseAddress);

					received.Set(message);
				});

			LocalBus.Publish(new PingMessage(), context => context.SendResponseTo(LocalBus));

			Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
		}

		[Test]
		public void The_source_address_should_pass()
		{
			var received = new FutureMessage<PingMessage>();

			LocalBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Address.Uri, LocalBus.Context().SourceAddress);

					received.Set(message);
				});

			LocalBus.Publish(new PingMessage());

			Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
		}
	}

	[TestFixture]
	public class When_publishing_a_message_with_no_consumers :
		LoopbackLocalAndRemoteTestFixture
	{
		[Test]
		public void The_method_should_be_called_to_notify_the_caller()
		{
			var ping = new PingMessage();

			bool noConsumers = false;

			LocalBus.Publish(ping, x =>
				{
					x.IfNoSubscribers(message =>
						{
							noConsumers = true;
						});
				});

			Assert.IsTrue(noConsumers, "There should have been no consumers");
		}

		[Test]
		public void The_method_should_not_carry_over_the_subsequent_calls()
		{
			var ping = new PingMessage();

			int hitCount = 0;

			LocalBus.Publish(ping, x => x.IfNoSubscribers(message => hitCount++));
			LocalBus.Publish(ping);

			Assert.AreEqual(1, hitCount, "There should have been no consumers");
		}
	}

	[TestFixture]
	public class When_publishing_a_message_with_an_each_consumer_action_specified :
		LoopbackLocalAndRemoteTestFixture
	{
		[Test]
		public void The_method_should_be_called_for_each_destination_endpoint()
		{
			LocalBus.SubscribeHandler<PingMessage>(x => { });

			var ping = new PingMessage();

			var consumers = new List<Uri>();

			LocalBus.Publish(ping, x => { x.ForEachSubscriber((message, endpoint) => consumers.Add(endpoint.Address.Uri)); });

			Assert.AreEqual(1, consumers.Count);
			Assert.AreEqual(LocalBus.Endpoint.Address.Uri, consumers[0]);
		}

		[Test]
		public void The_method_should_be_called_for_each_destination_endpoint_when_there_are_multiple()
		{
			LocalBus.SubscribeHandler<PingMessage>(x => { });
			RemoteBus.SubscribeHandler<PingMessage>(x => { });

			var ping = new PingMessage();

			var consumers = new List<Uri>();

			LocalBus.Publish(ping, x => { x.ForEachSubscriber((message, endpoint) => consumers.Add(endpoint.Address.Uri)); });

			Assert.AreEqual(2, consumers.Count);
			Assert.IsTrue(consumers.Contains(LocalBus.Endpoint.Address.Uri));
			Assert.IsTrue(consumers.Contains(RemoteBus.Endpoint.Address.Uri));
		}

		[Test]
		public void The_method_should_not_be_called_when_there_are_no_subscribers()
		{
			var ping = new PingMessage();

			var consumers = new List<Uri>();

			LocalBus.Publish(ping, x => { x.ForEachSubscriber((message, consumer) => consumers.Add(consumer.Address.Uri)); });

			Assert.AreEqual(0, consumers.Count);
		}

		[Test]
		public void The_method_should_not_carry_over_to_the_next_call_context()
		{
			var ping = new PingMessage();

			var consumers = new List<Uri>();

			LocalBus.Publish(ping, x => { x.ForEachSubscriber((message, endpoint) => consumers.Add(endpoint.Address.Uri)); });

			LocalBus.SubscribeHandler<PingMessage>(x => { });

			LocalBus.Publish(ping);

			Assert.AreEqual(0, consumers.Count);
		}
	}
}