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
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Internal;
	using Messages;
	using NUnit.Framework;
	using TestConsumers;
	using TextFixtures;

	[TestFixture]
	public class MessageContext_Specs :
		LoopbackLocalAndRemoteTestFixture
	{
		[Test]
		public void A_response_should_be_published_if_no_reply_address_is_specified()
		{
			PingMessage ping = new PingMessage();

			TestMessageConsumer<PongMessage> otherConsumer = new TestMessageConsumer<PongMessage>();
			RemoteBus.Subscribe(otherConsumer);

			TestCorrelatedConsumer<PongMessage, Guid> consumer = new TestCorrelatedConsumer<PongMessage, Guid>(ping.CorrelationId);
			LocalBus.Subscribe(consumer);

			FutureMessage<PongMessage> pong = new FutureMessage<PongMessage>();

			RemoteBus.Subscribe<PingMessage>(message =>
				{
					pong.Set(new PongMessage(message.CorrelationId));

					CurrentMessage.Respond(pong.Message);
				});

			LocalBus.Publish(ping);

			Assert.IsTrue(pong.IsAvailable(3.Seconds()), "No pong generated");

			consumer.ShouldHaveReceivedMessage(pong.Message, 3.Seconds());
			otherConsumer.ShouldHaveReceivedMessage(pong.Message, 1.Seconds());
		}

		[Test]
		public void A_response_should_be_sent_directly_if_a_reply_address_is_specified()
		{
			PingMessage ping = new PingMessage();

			TestMessageConsumer<PongMessage> otherConsumer = new TestMessageConsumer<PongMessage>();
			RemoteBus.Subscribe(otherConsumer);

			TestCorrelatedConsumer<PongMessage, Guid> consumer = new TestCorrelatedConsumer<PongMessage, Guid>(ping.CorrelationId);
			LocalBus.Subscribe(consumer);

			FutureMessage<PongMessage> pong = new FutureMessage<PongMessage>();

			RemoteBus.Subscribe<PingMessage>(message =>
				{
					pong.Set(new PongMessage(message.CorrelationId));

					CurrentMessage.Respond(pong.Message);
				});

			LocalBus.Publish(ping, context => context.SendResponseTo(LocalBus));

			Assert.IsTrue(pong.IsAvailable(3.Seconds()), "No pong generated");

			consumer.ShouldHaveReceivedMessage(pong.Message, 3.Seconds());
			otherConsumer.ShouldNotHaveReceivedMessage(pong.Message, 1.Seconds());
		}

		[Test]
		public void The_destination_address_should_pass()
		{
			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			LocalBus.Subscribe<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Uri, CurrentMessage.Headers.DestinationAddress);

					received.Set(message);
				});

			LocalBus.Publish(new PingMessage());

			Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
		}

		[Test]
		public void The_fault_address_should_pass()
		{
			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			LocalBus.Subscribe<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Uri, CurrentMessage.Headers.FaultAddress);

					received.Set(message);
				});

			LocalBus.Publish(new PingMessage(), context => context.SendFaultTo(LocalBus));

			Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
		}

		[Test]
		public void The_response_address_should_pass()
		{
			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			LocalBus.Subscribe<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Uri, CurrentMessage.Headers.ResponseAddress);

					received.Set(message);
				});

			LocalBus.Publish(new PingMessage(), context => context.SendResponseTo(LocalBus));

			Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
		}

		[Test]
		public void The_source_address_should_pass()
		{
			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			LocalBus.Subscribe<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Uri, CurrentMessage.Headers.SourceAddress);

					received.Set(message);
				});

			LocalBus.Publish(new PingMessage());

			Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
		}
	}
}