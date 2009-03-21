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
namespace MassTransit.Tests.Subscriptions
{
	using System.Threading;
	using Magnum.DateTimeExtensions;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestConsumers;
	using TextFixtures;

	[TestFixture]
	public class SubscriptionService_Specs :
		SubscriptionServiceTestFixture
	{
		[Test]
		public void The_initial_subscriptions_should_be_read_from_the_repository()
		{
			SubscriptionRepository.AssertWasCalled(x => x.List());
		}

		[Test]
		public void The_system_should_be_ready_to_use_before_getting_underway()
		{
			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe(consumer);

			Thread.Sleep(1000);

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, 500.Milliseconds());
		}
	}
}