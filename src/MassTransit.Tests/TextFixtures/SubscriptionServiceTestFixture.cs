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
namespace MassTransit.Tests.TextFixtures
{
	using System;
	using System.Collections.Generic;
	using BusConfigurators;
	using Distributor;
	using Magnum.Extensions;
	using MassTransit.Saga;
	using MassTransit.Services.Subscriptions.Server;
	using MassTransit.Transports;
	using NUnit.Framework;

	[TestFixture, Ignore]
	public class SubscriptionServiceTestFixture<TTransportFactory> :
		EndpointTestFixture<TTransportFactory>
		where TTransportFactory : class, ITransportFactory, new()
	{
		ISagaRepository<SubscriptionClientSaga> _subscriptionClientSagaRepository;
		ISagaRepository<SubscriptionSaga> _subscriptionSagaRepository;
		protected string SubscriptionServiceUri = "loopback://localhost/mt_subscriptions";
		protected string ClientControlUri = "loopback://localhost/mt_client_control";
		protected string ServerControlUri = "loopback://localhost/mt_server_control";
		protected string ClientUri = "loopback://localhost/mt_client";
		protected string ServerUri = "loopback://localhost/mt_server";
		protected SubscriptionService SubscriptionService { get; private set; }
		protected IServiceBus LocalBus { get; set; }
		protected IServiceBus LocalControlBus { get; set; }
		protected IServiceBus RemoteBus { get; set; }
		protected IServiceBus RemoteControlBus { get; private set; }
		protected IServiceBus SubscriptionBus { get; set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			SubscriptionBus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom(SubscriptionServiceUri);
					x.SetConcurrentConsumerLimit(1);
				});

			SetupSubscriptionService();

			SetupLocalBus();

			SetupRemoteBus();

			Instances = new Dictionary<string, ServiceInstance>();
		}

		protected void SetupLocalBus()
		{
			LocalBus = ServiceBusFactory.New(x =>
				{
					x.UseSubscriptionService(SubscriptionServiceUri);
					x.ReceiveFrom(ClientUri);
					x.SetConcurrentConsumerLimit(4);
					x.UseControlBus();

					ConfigureLocalBus(x);
				});

			LocalControlBus = LocalBus.ControlBus;
		}

		protected void SetupRemoteBus()
		{
			RemoteBus = ServiceBusFactory.New(x =>
				{
					x.UseSubscriptionService(SubscriptionServiceUri);
					x.ReceiveFrom(ServerUri);
					x.UseControlBus();

					ConfigureRemoteBus(x);
				});

			RemoteControlBus = RemoteBus.ControlBus;
		}

		protected Dictionary<string, ServiceInstance> Instances { get; private set; }

		protected ServiceInstance AddInstance(string instanceName, string queueName,
		                                      Action<ServiceBusConfigurator> configureBus)
		{
			var instance = new ServiceInstance(queueName, SubscriptionServiceUri, configureBus);

			Instances.Add(instanceName, instance);

			return instance;
		}

		protected virtual void ConfigureLocalBus(ServiceBusConfigurator configurator)
		{
		}

		protected virtual void ConfigureRemoteBus(ServiceBusConfigurator configurator)
		{
		}

		void SetupSubscriptionService()
		{
			_subscriptionClientSagaRepository = SetupSagaRepository<SubscriptionClientSaga>();

			_subscriptionSagaRepository = SetupSagaRepository<SubscriptionSaga>();

			SubscriptionService = new SubscriptionService(SubscriptionBus, _subscriptionSagaRepository,
				_subscriptionClientSagaRepository);

			SubscriptionService.Start();
		}


		protected override void TeardownContext()
		{
			Instances.Each(x => x.Value.Dispose());

			Instances.Clear();

			RemoteBus.Dispose();
			RemoteBus = null;
			RemoteControlBus = null;

			LocalBus.Dispose();
			LocalBus = null;
			LocalControlBus = null;

			SubscriptionService.Stop();
			SubscriptionService.Dispose();
			SubscriptionService = null;

			SubscriptionBus.Dispose();
			SubscriptionBus = null;

			base.TeardownContext();
		}
	}
}