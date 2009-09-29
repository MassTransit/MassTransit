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
namespace MassTransit.Transports.Nms.Tests
{
	using System;
	using Magnum.DateTimeExtensions;
	using MassTransit.Tests.Messages;
	using MassTransit.Tests.TestConsumers;
	using NUnit.Framework;
	using TestFixtures;

    [TestFixture, Category("Integration")]
	public class When_publishing_a_message :
		NmsEndpointTestFixture
	{
		private readonly TimeSpan _timeout = 10.Seconds();

		[Test]
		public void It_should_be_received_by_multiple_subscribed_consumers()
		{
			var localConsumer = new TestMessageConsumer<PingMessage>();
			LocalBus.Subscribe(localConsumer);

			var remoteConsumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe(remoteConsumer);

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);

			localConsumer.ShouldHaveReceivedMessage(message, _timeout);
			remoteConsumer.ShouldHaveReceivedMessage(message, _timeout);
		}

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
		public void It_should_be_received_by_one_subscribed_consumer_on_the_same_bus()
		{
			var consumer = new TestMessageConsumer<PingMessage>();
			LocalBus.Subscribe(consumer);

			var message = new PingMessage();
			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, _timeout);
		}
	}
}