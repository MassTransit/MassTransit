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
	using BusConfigurators;
	using Messages;
	using NUnit.Framework;
	using TestConsumers;
	using TextFixtures;
	using TestFramework;

	[TestFixture]
	public class When_a_request_message_is_published :
		LoopbackLocalAndRemoteTestFixture
	{
		static readonly TimeSpan _timeout = TimeSpan.FromSeconds(8);


		protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
		{
			base.ConfigureRemoteBus(configurator);

			configurator.Subscribe(x => x.Consumer<TestReplyService<PingMessage, Guid, PongMessage>>());
		}

		[Test]
		public void A_reply_should_be_received_by_the_requestor()
		{
			RemoteBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
			LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();

			var message = new PingMessage();

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(message.CorrelationId);
			LocalBus.SubscribeInstance(consumer);

			RemoteBus.ShouldHaveRemoteSubscriptionFor<PongMessage>();

			LocalBus.Publish(message);

			TestConsumerBase<PingMessage>.AnyShouldHaveReceivedMessage(message, _timeout);

			consumer.ShouldHaveReceivedMessage(new PongMessage(message.CorrelationId), _timeout);
		}
	}
}