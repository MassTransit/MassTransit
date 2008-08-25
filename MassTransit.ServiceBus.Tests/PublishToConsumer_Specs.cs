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
namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class When_a_message_is_published :
		LocalAndRemoteTestContext
	{
		private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);

		[Test]
		public void It_should_be_ignored_if_there_are_no_consumers()
		{
			PingMessage message = new PingMessage();

			LocalBus.Publish(message);

		}

		[Test]
		public void It_should_be_received_by_an_interested_correlated_consumer()
		{
			PingMessage message = new PingMessage();

			TestCorrelatedConsumer<PingMessage, Guid> consumer = new TestCorrelatedConsumer<PingMessage, Guid>(message.CorrelationId);
			RemoteBus.Subscribe(consumer);

			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, _timeout);
		}

		[Test]
		public void It_should_be_received_by_multiple_subscribed_consumers()
		{
			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe(consumer);

			TestMessageConsumer<PingMessage> messageConsumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe<PingMessage>(messageConsumer.MessageHandler);

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, _timeout);
			messageConsumer.ShouldHaveReceivedMessage(message, _timeout);
		}

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
		public void It_should_be_received_by_a_component()
		{
			RemoteBus.AddComponent<TestMessageConsumer<PingMessage>>();

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);

			TestMessageConsumer<PingMessage>.AnyShouldHaveReceivedMessage(message, _timeout);
		}

		[Test]
		public void It_should_be_received_by_one_subscribed_message_handler()
		{
			TestMessageConsumer<PingMessage> messageConsumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe<PingMessage>(messageConsumer.MessageHandler);

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);

			messageConsumer.ShouldHaveReceivedMessage(message, _timeout);
		}

		[Test]
		public void It_should_not_be_received_by_an_uninterested_consumer()
		{
			TestMessageConsumer<PingMessage> messageConsumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe<PingMessage>(messageConsumer.MessageHandler, x => false);

			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe(consumer);

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, _timeout);
			messageConsumer.ShouldNotHaveReceivedMessage(message, TimeSpan.FromSeconds(1));
		}

		[Test]
		public void It_should_not_be_received_by_an_uninterested_correlated_consumer()
		{
			TestCorrelatedConsumer<PingMessage, Guid> consumer = new TestCorrelatedConsumer<PingMessage, Guid>(Guid.NewGuid());
			RemoteBus.Subscribe(consumer);

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);

			consumer.ShouldNotHaveReceivedMessage(message, _timeout);
		}
	}
}