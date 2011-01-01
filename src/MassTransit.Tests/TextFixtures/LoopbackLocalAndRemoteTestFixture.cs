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
namespace MassTransit.Tests.TextFixtures
{
	using Configuration;
	using MassTransit.Internal;
	using MassTransit.Services.Subscriptions;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class LoopbackLocalAndRemoteTestFixture :
		EndpointTestFixture<LoopbackEndpoint>
	{
		public ISubscriptionService SubscriptionService { get; private set; }
		public IServiceBus LocalBus { get; private set; }
		public IServiceBus RemoteBus { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			SetupSubscriptionService();

			LocalBus = ServiceBusConfigurator.New(x =>
				{
					x.AddService<SubscriptionPublisher>();
					x.AddService<SubscriptionConsumer>();
					x.ReceiveFrom("loopback://localhost/mt_client");
				});

			RemoteBus = ServiceBusConfigurator.New(x =>
				{
					x.AddService<SubscriptionPublisher>();
					x.AddService<SubscriptionConsumer>();
					x.ReceiveFrom("loopback://localhost/mt_server");
				});
		}

		private void SetupSubscriptionService()
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
						invocation.ReturnValue = new SubscriptionConsumer(SubscriptionService, EndpointResolver);
					});
		}

		protected override void TeardownContext()
		{
			RemoteBus.Dispose();
			RemoteBus = null;

			LocalBus.Dispose();
			LocalBus = null;

			SubscriptionService = null;

			base.TeardownContext();
		}
	}
}