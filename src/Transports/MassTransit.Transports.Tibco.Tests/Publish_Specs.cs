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
namespace MassTransit.Transports.Tibco.Tests
{
	using System;
	using MassTransit.Tests;
	using MassTransit.Tests.Messages;
	using MassTransit.Tests.TestConsumers;
	using NUnit.Framework;
	using TestFixtures;

	[TestFixture]
	public class Publish_Specs :
		TibcoEndpointTestFixture
	{
		private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

		[Test]
		public void It_should_be_received_by_one_subscribed_consumer()
		{
			var consumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe(consumer);

			var message = new PingMessage();
			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, _timeout);
		}

		[Test]
		public void Echo_reply_should_work()
		{
			var echoConsumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe(echoConsumer);

			var replyConsumer = new TestMessageConsumer<PongMessage>();
			LocalBus.Subscribe(replyConsumer);

			var echoMessage = new PingMessage();
			LocalBus.Publish(echoMessage);

			echoConsumer.ShouldHaveReceivedMessage(echoMessage, _timeout);

			PongMessage replyMessage = new PongMessage(echoMessage.CorrelationId);
			RemoteBus.Publish(replyMessage);

			replyConsumer.ShouldHaveReceivedMessage(replyMessage, _timeout);
		}

		[Test]
		public void It_should_leave_the_message_in_the_queue_if_an_exception_is_thrown()
		{
			var consumer = new TestSelectiveConsumer<PingMessage>(x => false);
			RemoteBus.Subscribe(consumer);

			var realConsumer = new TestMessageConsumer<PingMessage>();
			LocalBus.Subscribe(realConsumer);

			var message = new PingMessage();
			LocalBus.Publish(message);

			realConsumer.ShouldHaveReceivedMessage(message, _timeout);
			consumer.ShouldNotHaveReceivedMessage(message, _timeout);
		}
	}
}