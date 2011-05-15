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
	using Magnum.Extensions;
	using Messages;
	using NUnit.Framework;
	using TestFramework;
	using TextFixtures;

	[TestFixture]
	public class When_a_message_consumer_specifies_that_it_should_retry_a_message :
		LoopbackTestFixture
	{
		[Test]
		public void The_retry_count_should_be_set_on_the_message()
		{
			var future = new FutureMessage<PingMessage>();

			bool first = true;

			LocalBus.SubscribeHandler<PingMessage>(message =>
				{
					if (first)
					{
						Assert.AreEqual(0, LocalBus.Context().RetryCount);

						LocalBus.Context().RetryLater();

						first = false;
					}
					else
					{
						Assert.AreEqual(1, LocalBus.Context().RetryCount);

						future.Set(message);
					}
				});

			LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();

			LocalBus.Publish(new PingMessage());

			Assert.IsTrue(future.IsAvailable(20.Seconds()));
		}
	}
}