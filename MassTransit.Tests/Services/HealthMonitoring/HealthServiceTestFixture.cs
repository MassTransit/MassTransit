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
namespace MassTransit.Tests.Services.HealthMonitoring
{
	using System.Collections.Generic;
	using System.Threading;
	using Configuration;
	using MassTransit.Saga;
	using MassTransit.Services.HealthMonitoring;
	using MassTransit.Services.HealthMonitoring.Messages;
	using MassTransit.Services.HealthMonitoring.Server;
	using MassTransit.Services.Subscriptions;
	using MassTransit.Services.Subscriptions.Client;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Services.Subscriptions.Messages;
	using MassTransit.Services.Subscriptions.Server;
	using MassTransit.Services.Subscriptions.Server.Messages;
	using MassTransit.Services.Timeout.Messages;
	using MassTransit.Transports;
	using Microsoft.Practices.ServiceLocation;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TextFixtures;

	[TestFixture]
	public class HealthServiceTestFixture :
		EndpointTestFixture<LoopbackEndpoint>
	{
		private ISagaRepository<HealthSaga> _healthSagaRepository;
		private ISagaRepository<SubscriptionSaga> _subscriptionSagaRepository;
		public ISubscriptionRepository SubscriptionRepository { get; private set; }
		private ISagaRepository<SubscriptionClientSaga> _subscriptionClientSagaRepository;
		public IServiceBus SubscriptionBus { get; private set; }
		public SubscriptionService SubscriptionService { get; private set; }
		public HealthService HealthService { get; private set; }
		public IServiceBus LocalBus { get; private set; }
		public IServiceBus RemoteBus { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			ServiceLocator.SetLocatorProvider(() => ObjectBuilder);
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

			SetupHealthService();

			Thread.Sleep(1000);
		}

		private void SetupSubscriptionService()
		{
			//SubscriptionRepository = new InMemorySubscriptionRepository();
			SubscriptionRepository = MockRepository.GenerateMock<ISubscriptionRepository>();
			SubscriptionRepository.Expect(x => x.List()).Return(new List<Subscription>());
			ObjectBuilder.Stub(x => x.GetInstance<ISubscriptionRepository>())
				.Return(SubscriptionRepository);

			_subscriptionClientSagaRepository = SetupSagaRepository<SubscriptionClientSaga>();
			SetupInitiateSagaStateMachineSink<SubscriptionClientSaga, AddSubscriptionClient>(SubscriptionBus, _subscriptionClientSagaRepository);
			SetupOrchestrateSagaStateMachineSink<SubscriptionClientSaga, RemoveSubscriptionClient>(SubscriptionBus, _subscriptionClientSagaRepository);
			SetupObservesSagaStateMachineSink<SubscriptionClientSaga, SubscriptionClientAdded>(SubscriptionBus, _subscriptionClientSagaRepository);

			_subscriptionSagaRepository = SetupSagaRepository<SubscriptionSaga>();
			SetupInitiateSagaStateMachineSink<SubscriptionSaga, AddSubscription>(SubscriptionBus, _subscriptionSagaRepository);
			SetupOrchestrateSagaStateMachineSink<SubscriptionSaga, RemoveSubscription>(SubscriptionBus, _subscriptionSagaRepository);
			SetupObservesSagaStateMachineSink<SubscriptionSaga, SubscriptionClientRemoved>(SubscriptionBus, _subscriptionSagaRepository);

			SubscriptionService = new SubscriptionService(SubscriptionBus, SubscriptionRepository, EndpointFactory, _subscriptionSagaRepository, _subscriptionClientSagaRepository);

			SubscriptionService.Start();

			ObjectBuilder.Stub(x => x.GetInstance<SubscriptionClient>())
				.Return(null)
				.WhenCalled(invocation => { invocation.ReturnValue = new SubscriptionClient(EndpointFactory); });
		}

		private void SetupHealthService()
		{
			_healthSagaRepository = SetupSagaRepository<HealthSaga>();
			SetupInitiateSagaStateMachineSink<HealthSaga, EndpointCameOnline>(RemoteBus, _healthSagaRepository);
			SetupOrchestrateSagaStateMachineSink<HealthSaga, EndpointWentOffline>(RemoteBus, _healthSagaRepository);
			SetupOrchestrateSagaStateMachineSink<HealthSaga, TimeoutExpired>(RemoteBus, _healthSagaRepository);
			SetupOrchestrateSagaStateMachineSink<HealthSaga, PingEndpointResponse>(RemoteBus, _healthSagaRepository);
			SetupObservesSagaStateMachineSink<HealthSaga, Heartbeat>(RemoteBus, _healthSagaRepository);

			HealthService = new HealthService(RemoteBus, _healthSagaRepository, ObjectBuilder);

			HealthService.Start();
		}

		public ISagaRepository<HealthSaga> Repository
		{
			get { return _healthSagaRepository; }
		}

		protected override void TeardownContext()
		{
			RemoteBus.Dispose();
			RemoteBus = null;

			LocalBus.Dispose();
			LocalBus = null;

			Thread.Sleep(500);

			HealthService.Stop();
			HealthService.Dispose();
			HealthService = null;

			base.TeardownContext();
		}
	}
}