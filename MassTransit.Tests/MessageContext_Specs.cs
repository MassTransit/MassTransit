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

					CurrentMessage.Reply(pong.Message);
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

				CurrentMessage.Reply(pong.Message);
			});

			LocalBus.Publish(ping, x => x.SendReplyTo(LocalBus));

			Assert.IsTrue(pong.IsAvailable(3.Seconds()), "No pong generated");

			consumer.ShouldHaveReceivedMessage(pong.Message, 3.Seconds());
			otherConsumer.ShouldNotHaveReceivedMessage(pong.Message, 1.Seconds());
		}
	}
}