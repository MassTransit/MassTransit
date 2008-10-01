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
	public class When_publishing_a_message_via_multicast :
		LocalAndRemoteTestContext
	{
		private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

		protected override string GetCastleConfigurationFile()
		{
			return "multicast.castle.xml";
		}

		[Test]
		public void It_should_be_received()
		{
			PingMessage message = new PingMessage();

			TestCorrelatedConsumer<PingMessage, Guid> consumer = new TestCorrelatedConsumer<PingMessage, Guid>(message.CorrelationId);
			RemoteBus.Subscribe(consumer);

			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, _timeout);
		}

		[Test]
		public void It_should_be_received_by_both_receivers()
		{
			PingMessage message = new PingMessage();

			TestCorrelatedConsumer<PingMessage, Guid> remoteConsumer = new TestCorrelatedConsumer<PingMessage, Guid>(message.CorrelationId);
			RemoteBus.Subscribe(remoteConsumer);

			TestCorrelatedConsumer<PingMessage, Guid> localConsumer = new TestCorrelatedConsumer<PingMessage, Guid>(message.CorrelationId);
			LocalBus.Subscribe(localConsumer);

			// okay so a shared endpoint results in only one service bus in the process getting the message

			LocalBus.Publish(message);
			LocalBus.Publish(message);
			LocalBus.Publish(message);
			LocalBus.Publish(message);
			LocalBus.Publish(message);
			LocalBus.Publish(message);
			LocalBus.Publish(message);
			LocalBus.Publish(message);
			LocalBus.Publish(message);
			LocalBus.Publish(message);
			LocalBus.Publish(message);

			remoteConsumer.ShouldHaveReceivedMessage(message, _timeout);
			localConsumer.ShouldHaveReceivedMessage(message, _timeout);
		}
	}
}