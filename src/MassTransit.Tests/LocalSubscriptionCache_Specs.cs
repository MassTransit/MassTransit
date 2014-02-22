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
	using MassTransit.Services.Subscriptions;
	using MassTransit.Transports.Loopback;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestConsumers;
	using TextFixtures;

	[TestFixture]
	public class When_a_handler_subscription_is_added :
		EndpointTestFixture<LoopbackTransportFactory>
	{
		ISubscriptionService _subscriptionService;

		public IServiceBus LocalBus { get; private set; }
		public IServiceBus LocalControlBus { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_subscriptionService = MockRepository.GenerateMock<ISubscriptionService>();

			LocalBus = ServiceBusFactory.New(x =>
				{
					x.AddService(BusServiceLayer.Session, () => new SubscriptionPublisher(_subscriptionService));
					x.ReceiveFrom("loopback://localhost/mt_client");
					x.UseControlBus();
				});

			LocalControlBus = LocalBus.ControlBus;
		}

		protected override void TeardownContext()
		{
			LocalBus.Dispose();
			LocalBus = null;

			LocalControlBus.Dispose();
			LocalControlBus = null;

			base.TeardownContext();
		}

		[Test]
		public void A_subscription_should_be_added_for_a_consumer()
		{
			var consumer = new TestMessageConsumer<PingMessage>();

			LocalBus.SubscribeInstance(consumer);

			_subscriptionService.AssertWasCalled(x => x.SubscribedTo<PingMessage>(LocalBus.Endpoint.Address.Uri));
		}


		[Test]
		public void The_bus_should_add_a_subscription_to_the_subscription_cache()
		{
			LocalBus.SubscribeHandler<PingMessage>(delegate { });

			_subscriptionService.AssertWasCalled(x => x.SubscribedTo<PingMessage>(LocalBus.Endpoint.Address.Uri));
		}
	}
}