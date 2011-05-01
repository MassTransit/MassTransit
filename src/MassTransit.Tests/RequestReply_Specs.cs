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
	using Messages;
	using NUnit.Framework;
	using TestConsumers;
	using TextFixtures;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_request_message_is_published :
		LoopbackLocalAndRemoteTestFixture
	{
		private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);


		protected override void ConfigureRemoteBus(BusConfigurators.ServiceBusConfigurator configurator)
		{
			base.ConfigureRemoteBus(configurator);

			configurator.Subscribe(x => x.Consumer<TestReplyService<PingMessage,Guid, PongMessage>>());
		}
		[Test]
		public void A_reply_should_be_received_by_the_requestor()
		{
			RemoteBus.SubscribeConsumer<TestReplyService<PingMessage, Guid, PongMessage>>();

			PingMessage message = new PingMessage();

			TestCorrelatedConsumer<PongMessage, Guid> consumer = new TestCorrelatedConsumer<PongMessage, Guid>(message.CorrelationId);
			LocalBus.SubscribeInstance(consumer);

			LocalBus.Publish(message);

			TestConsumerBase<PingMessage>.AnyShouldHaveReceivedMessage(message, _timeout);

			consumer.ShouldHaveReceivedMessage(new PongMessage(message.CorrelationId), _timeout);
		}
	}
}