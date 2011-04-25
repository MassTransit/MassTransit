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
	using System.Threading;
	using Configuration;
	using MassTransit.Saga;
	using MassTransit.Services.HealthMonitoring;
	using MassTransit.Services.HealthMonitoring.Messages;
	using MassTransit.Services.HealthMonitoring.Server;
	using MassTransit.Services.Subscriptions.Client;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Services.Subscriptions.Server;
	using MassTransit.Services.Timeout.Messages;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TextFixtures;
	using TestFramework;

	[TestFixture]
	public class HealthServiceTestFixture :
		EndpointTestFixture<LoopbackTransportFactory>
	{
		private ISagaRepository<HealthSaga> _healthSagaRepository;
		private ISagaRepository<SubscriptionSaga> _subscriptionSagaRepository;
		private ISagaRepository<SubscriptionClientSaga> _subscriptionClientSagaRepository;
		public IServiceBus SubscriptionBus { get; private set; }
		public SubscriptionService SubscriptionService { get; private set; }
		public HealthService HealthService { get; private set; }
		public IServiceBus LocalBus { get; private set; }
		public IServiceBus RemoteBus { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			const string subscriptionServiceEndpointAddress = "loopback://localhost/mt_subscriptions";

			SubscriptionBus = ServiceBusFactory.New(x => { x.ReceiveFrom(subscriptionServiceEndpointAddress); });

			SetupSubscriptionService();

			LocalBus = ServiceBusFactory.New(x =>
				{
					x.ConfigureService<SubscriptionClientConfigurator>(y =>
						{
							// setup endpoint
							y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
						});
					x.ReceiveFrom("loopback://localhost/mt_client");
				});

			RemoteBus = ServiceBusFactory.New(x =>
				{
					x.ConfigureService<SubscriptionClientConfigurator>(y =>
						{
							// setup endpoint
							y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
						});
					x.ReceiveFrom("loopback://localhost/mt_server");
				});

			SetupHealthService();

			Thread.Sleep(500);
		}

		private void SetupSubscriptionService()
		{
			_subscriptionClientSagaRepository = SetupSagaRepository<SubscriptionClientSaga>(ObjectBuilder);

			_subscriptionSagaRepository = SetupSagaRepository<SubscriptionSaga>(ObjectBuilder);

			SubscriptionService = new SubscriptionService(SubscriptionBus, EndpointCache, _subscriptionSagaRepository, _subscriptionClientSagaRepository);

			SubscriptionService.Start();

		}

		private void SetupHealthService()
		{
			_healthSagaRepository = SetupSagaRepository<HealthSaga>(ObjectBuilder);

			HealthService = new HealthService(RemoteBus, _healthSagaRepository);

			HealthService.Start();

			LocalBus.ShouldHaveSubscriptionFor<EndpointCameOnline>();
			LocalBus.ShouldHaveSubscriptionFor<Heartbeat>();
			LocalBus.ShouldHaveSubscriptionFor<EndpointWentOffline>();
			LocalBus.ShouldHaveSubscriptionFor<TimeoutExpired>();
			LocalBus.ShouldHaveSubscriptionFor<PingEndpointResponse>();
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

			SubscriptionService.Stop();
			SubscriptionService.Dispose();
			SubscriptionService = null;

			base.TeardownContext();
		}
	}
}