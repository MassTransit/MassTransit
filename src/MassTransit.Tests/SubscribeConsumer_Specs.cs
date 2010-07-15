// Copyright 2007-2010 The Apache Software Foundation.
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
	using Magnum.Extensions;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TextFixtures;

	[TestFixture]
	public class A_subscribed_consumer
		: LoopbackTestFixture
	{
		[Test]
		public void Should_request_an_instance_of_the_consumer_for_each_message()
		{
			var called = new FutureMessage<PingMessage>();

			var ping = new PingMessage();

			var getter = MockRepository.GenerateMock<Func<PingMessage, Action<PingMessage>>>();
			getter.Expect(x => x(ping)).Return(called.Set);

			LocalBus.SubscribeConsumer<PingMessage>(getter);

			LocalBus.Publish(ping);

			called.IsAvailable(3.Seconds()).ShouldBeTrue();
		}
	}
}
