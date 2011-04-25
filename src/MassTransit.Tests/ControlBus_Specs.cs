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
	using MassTransit.Internal;
	using MassTransit.Services.Subscriptions;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TextFixtures;

	[TestFixture]
	public class When_creating_a_bus_with_a_separate_control_bus :
		EndpointTestFixture<LoopbackTransportFactory>
	{
		public ISubscriptionService SubscriptionService { get; private set; }
		public IServiceBus LocalBus { get; private set; }
		public IServiceBus LocalControlBus { get; private set; }
		public IServiceBus RemoteBus { get; private set; }
		public IServiceBus RemoteControlBus { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			SetupSubscriptionService();

			LocalBus = ServiceBusFactory.New(x =>
				{
					x.AddService(() => new SubscriptionPublisher(SubscriptionService));
					x.AddService(() => new SubscriptionConsumer(SubscriptionService));
					x.ReceiveFrom("loopback://localhost/mt_client");
					x.UseControlBus();
				});

			RemoteBus = ServiceBusFactory.New(x =>
				{
					x.AddService(() => new SubscriptionPublisher(SubscriptionService));
					x.AddService(() => new SubscriptionConsumer(SubscriptionService));
					x.ReceiveFrom("loopback://localhost/mt_server");
					x.UseControlBus();
				});

			LocalControlBus = LocalBus.ControlBus;
			RemoteControlBus = RemoteBus.ControlBus;
		}

		protected override void TeardownContext()
		{
			RemoteBus.Dispose();
			RemoteBus = null;

			RemoteControlBus.Dispose();
			RemoteControlBus = null;

			LocalBus.Dispose();
			LocalBus = null;

			LocalControlBus.Dispose();
			LocalControlBus = null;

			SubscriptionService = null;

			base.TeardownContext();
		}

		void SetupSubscriptionService()
		{
			SubscriptionService = new LocalSubscriptionService();
			ObjectBuilder.Stub(x => x.GetInstance<IEndpointSubscriptionEvent>())
				.Return(SubscriptionService);

			ObjectBuilder.Stub(x => x.GetInstance<SubscriptionPublisher>())
				.Return(null)
				.WhenCalled(invocation =>
					{
						// Return a unique instance of this class
						invocation.ReturnValue = new SubscriptionPublisher(SubscriptionService);
					});

			ObjectBuilder.Stub(x => x.GetInstance<SubscriptionConsumer>())
				.Return(null)
				.WhenCalled(invocation =>
					{
						// Return a unique instance of this class
						invocation.ReturnValue = new SubscriptionConsumer(SubscriptionService);
					});
		}

		[Test]
		public void Should_purge_messages_on_startup_if_specified()
		{
		}
	}
}