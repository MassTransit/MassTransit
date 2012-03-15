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
	using MassTransit.Transports.Loopback;
	using NUnit.Framework;
	using TestFramework;
	using TextFixtures;

	[TestFixture]
	public class Adding_a_correlated_subscription_via_the_subscription_client :
		SubscriptionServiceTestFixture<LoopbackTransportFactory>
	{
		const string FirstCorrelationId = "FirstId";
		const string SecondCorrelationId = "SecondId";

		public class FirstComponent :
			Consumes<IncomingMessage>.For<string>
		{
			public void Consume(IncomingMessage message)
			{
			}

			public string CorrelationId
			{
				get { return FirstCorrelationId; }
			}
		}

		public class SecondComponent :
			Consumes<IncomingMessage>.For<string>
		{
			public void Consume(IncomingMessage message)
			{
			}

			public string CorrelationId
			{
				get { return SecondCorrelationId; }
			}
		}

		public class IncomingMessage :
			CorrelatedBy<string>
		{
			public string CorrelationId { get; set; }
		}

		[Test]
		public void Should_properly_register_the_consumers_for_each_endpoint()
		{
			var firstComponent = new FirstComponent();
			var unsubFirst = LocalBus.SubscribeInstance(firstComponent);

			var secondComponent = new SecondComponent();
			var unsubSecond = LocalBus.SubscribeInstance(secondComponent);

			RemoteBus.ShouldHaveSubscriptionFor<IncomingMessage>();

			RemoteBus.ShouldHaveCorrelatedSubscriptionFor<IncomingMessage, string>(FirstCorrelationId);
			RemoteBus.ShouldHaveCorrelatedSubscriptionFor<IncomingMessage, string>(SecondCorrelationId);

		    unsubFirst();
		    unsubSecond();
		}
	}
}