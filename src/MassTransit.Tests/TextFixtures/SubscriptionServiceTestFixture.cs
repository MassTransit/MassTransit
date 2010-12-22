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
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Configuration;
	using Distributor;
	using MassTransit.Saga;
	using MassTransit.Services.Subscriptions;
	using MassTransit.Services.Subscriptions.Client;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Services.Subscriptions.Server;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class SubscriptionServiceTestFixture<TEndpoint> :
		EndpointTestFixture<TEndpoint>
		where TEndpoint : IEndpoint
	{
		private ISagaRepository<SubscriptionClientSaga> _subscriptionClientSagaRepository;
		private ISagaRepository<SubscriptionSaga> _subscriptionSagaRepository;
		public string SubscriptionServiceUri = "loopback://localhost/mt_subscriptions";
		public string ClientControlUri = "loopback://localhost/mt_client_control";
		public string ServerControlUri = "loopback://localhost/mt_server_control";
		public string ClientUri = "loopback://localhost/mt_client";
		public string ServerUri = "loopback://localhost/mt_server";
		public SubscriptionService SubscriptionService { get; private set; }
		public IServiceBus LocalBus { get; private set; }
		public IControlBus LocalControlBus { get; private set; }
		public IServiceBus RemoteBus { get; private set; }
		public IControlBus RemoteControlBus { get; private set; }
		public IServiceBus SubscriptionBus { get; private set; }
		public ISubscriptionRepository SubscriptionRepository { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			SubscriptionBus = ServiceBusConfigurator.New(x =>
				{
					x.ReceiveFrom(SubscriptionServiceUri);
					x.SetConcurrentConsumerLimit(1);
				});

			SetupSubscriptionService(ObjectBuilder);

			SetupLocalBus();

			SetupRemoteBus();

			Instances = new Dictionary<string, ServiceInstance>();
		}

		protected void SetupLocalBus()
		{
			LocalControlBus = ControlBusConfigurator.New(x =>
				{
					x.ReceiveFrom(ClientControlUri);

					x.PurgeBeforeStarting();
				});

			LocalBus = ServiceBusConfigurator.New(x =>
				{
					x.ConfigureService<SubscriptionClientConfigurator>(y =>
						{
							// setup endpoint
							y.SetSubscriptionServiceEndpoint(SubscriptionServiceUri);
						});
					x.ReceiveFrom(ClientUri);
					x.SetConcurrentConsumerLimit(4);
					x.UseControlBus(LocalControlBus);

					ConfigureLocalBus(x);
				});
		}

		protected void SetupRemoteBus()
		{
			RemoteControlBus = ControlBusConfigurator.New(x =>
				{
					x.ReceiveFrom(ServerControlUri);

					x.PurgeBeforeStarting();
				});

			RemoteBus = ServiceBusConfigurator.New(x =>
				{
					x.ConfigureService<SubscriptionClientConfigurator>(y =>
						{
							// setup endpoint
							y.SetSubscriptionServiceEndpoint(SubscriptionServiceUri);
						});
					x.ReceiveFrom(ServerUri);
					x.UseControlBus(RemoteControlBus);
				});
		}

		protected Dictionary<string, ServiceInstance> Instances { get; private set; }

		protected ServiceInstance AddInstance(string instanceName, string queueName, Action<IObjectBuilder> configureBuilder, Action<IServiceBusConfigurator> configureBus)
		{
			var instance = new ServiceInstance(queueName, EndpointFactory, SubscriptionServiceUri, configureBuilder, configureBus);

			Instances.Add(instanceName, instance);

			return instance;
		}

		protected virtual void ConfigureLocalBus(IServiceBusConfigurator configurator)
		{
		}

		private void SetupSubscriptionService(IObjectBuilder builder)
		{
			//SubscriptionRepository = new InMemorySubscriptionRepository();
			SubscriptionRepository = MockRepository.GenerateMock<ISubscriptionRepository>();
			SubscriptionRepository.Expect(x => x.List()).Return(new List<Subscription>());
			builder.Stub(x => x.GetInstance<ISubscriptionRepository>())
				.Return(SubscriptionRepository);

			_subscriptionClientSagaRepository = SetupSagaRepository<SubscriptionClientSaga>(builder);
			
			_subscriptionSagaRepository = SetupSagaRepository<SubscriptionSaga>(builder);
			
			SubscriptionService = new SubscriptionService(SubscriptionBus, SubscriptionRepository, EndpointFactory, _subscriptionSagaRepository, _subscriptionClientSagaRepository);

			SubscriptionService.Start();

			builder.Stub(x => x.GetInstance<SubscriptionClient>())
				.Return(null)
				.WhenCalled(invocation => { invocation.ReturnValue = new SubscriptionClient(EndpointFactory); });
		}


		protected override void TeardownContext()
		{
			Instances.Each(x => x.Value.Dispose());

			Instances.Clear();

			RemoteBus.Dispose();
			RemoteBus = null;

			RemoteControlBus.Dispose();
			RemoteControlBus = null;

			LocalBus.Dispose();
			LocalBus = null;

			LocalControlBus.Dispose();
			LocalControlBus = null;

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