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
	using System.Collections.Generic;
	using System.Threading;
	using Configuration;
	using MassTransit.Saga;
	using MassTransit.Services.Subscriptions;
	using MassTransit.Services.Subscriptions.Client;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Services.Subscriptions.Messages;
	using MassTransit.Services.Subscriptions.Server;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class SubscriptionServiceTestFixture :
		EndpointTestFixture<LoopbackEndpoint>
	{
		private ISagaRepository<SubscriptionClientSaga> _subscriptionClientSagaRepository;
		private ISagaRepository<SubscriptionSaga> _subscriptionSagaRepository;
		public SubscriptionService SubscriptionService { get; private set; }
		public IServiceBus LocalBus { get; private set; }
		public IServiceBus RemoteBus { get; private set; }
		public IServiceBus SubscriptionBus { get; private set; }
		public ISubscriptionRepository SubscriptionRepository { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			const string subscriptionServiceEndpointAddress = "loopback://localhost/mt_subscriptions";

			SubscriptionBus = ServiceBusConfigurator.New(x => { x.ReceiveFrom(subscriptionServiceEndpointAddress); });

			SetupSubscriptionService();

			LocalBus = ServiceBusConfigurator.New(x =>
				{
					x.ConfigureService<SubscriptionClientConfigurator>(y =>
						{
							// setup endpoint
							y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
						});
					x.ReceiveFrom("loopback://localhost/mt_client");
				});

			RemoteBus = ServiceBusConfigurator.New(x =>
				{
					x.ConfigureService<SubscriptionClientConfigurator>(y =>
						{
							// setup endpoint
							y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
						});
					x.ReceiveFrom("loopback://localhost/mt_server");
				});
		}

		private void SetupSubscriptionService()
		{
			//SubscriptionRepository = new InMemorySubscriptionRepository();
			SubscriptionRepository = MockRepository.GenerateMock<ISubscriptionRepository>();
			SubscriptionRepository.Expect(x => x.List()).Return(new List<Subscription>());
			ObjectBuilder.Stub(x => x.GetInstance<ISubscriptionRepository>())
				.Return(SubscriptionRepository);

			_subscriptionClientSagaRepository = SetupSagaRepository<SubscriptionClientSaga>();
			SetupInitiateSagaSink<SubscriptionClientSaga, CacheUpdateRequest>(SubscriptionBus, _subscriptionClientSagaRepository);
			SetupOrchestrateSagaSink<SubscriptionClientSaga, CancelSubscriptionUpdates>(SubscriptionBus, _subscriptionClientSagaRepository);

			_subscriptionSagaRepository = SetupSagaRepository<SubscriptionSaga>();
			SetupInitiateSagaSink<SubscriptionSaga, AddSubscription>(SubscriptionBus, _subscriptionSagaRepository);
			SetupOrchestrateSagaSink<SubscriptionSaga, RemoveSubscription>(SubscriptionBus, _subscriptionSagaRepository);

			SubscriptionService = new SubscriptionService(SubscriptionBus, SubscriptionRepository, EndpointFactory, _subscriptionSagaRepository, _subscriptionClientSagaRepository);

            SubscriptionService.Start();

			ObjectBuilder.Stub(x => x.GetInstance<SubscriptionClient>())
				.Return(null)
				.WhenCalled(invocation => { invocation.ReturnValue = new SubscriptionClient(EndpointFactory); });
		}


		protected override void TeardownContext()
		{
			RemoteBus.Dispose();
			RemoteBus = null;

			LocalBus.Dispose();
			LocalBus = null;

			Thread.Sleep(500);

			SubscriptionService.Stop();
			SubscriptionService.Dispose();
			SubscriptionService = null;

			SubscriptionBus.Dispose();
			SubscriptionBus = null;

			base.TeardownContext();
		}
	}
}