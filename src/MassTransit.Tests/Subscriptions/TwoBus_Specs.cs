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
namespace MassTransit.Tests.Subscriptions
{
	using System;
	using Examples.Messages;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using MassTransit.Transports.Loopback;
	using TestFramework;
	using TestFramework.Fixtures;

	[Scenario]
	public class When_publishing_to_a_remote_subscriber :
		TwoBusTestFixture<LoopbackTransportFactory>
	{
		SimpleMessage _message;
		UnsubscribeAction _unsubscribeAction;

		public When_publishing_to_a_remote_subscriber()
		{
			LocalUri = new Uri("loopback://localhost/mt_client");
			RemoteUri = new Uri("loopback://localhost/mt_server");
		}

		[When]
		public void Publishing_to_a_remote_subscriber()
		{
			Consumer = new ConsumerOf<SimpleMessage>();
			_unsubscribeAction = RemoteBus.SubscribeInstance(Consumer);

			RemoteBus.ShouldHaveSubscriptionFor<SimpleMessage>();
			
			LocalBus.ShouldHaveSubscriptionFor<SimpleMessage>();

			_message = new SimpleMessage();
			LocalBus.Publish(_message);
		}

		[Finally]
		public void Finally()
		{
			_unsubscribeAction();
		}

		protected ConsumerOf<SimpleMessage> Consumer { get; private set; }

		[Then]
		public void The_consumer_should_receive_the_message()
		{
			Consumer.ShouldHaveReceived(_message, 8.Seconds());
		}
	}
}