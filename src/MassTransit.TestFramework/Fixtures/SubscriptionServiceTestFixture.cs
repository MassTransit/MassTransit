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
namespace MassTransit.TestFramework.Fixtures
{
	using System;
	using System.Collections.Generic;
	using Configuration;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Saga;
	using Services.Subscriptions;
	using Services.Subscriptions.Client;
	using Services.Subscriptions.Configuration;
	using Services.Subscriptions.Server;

	[TestFixture]
	public class SubscriptionServiceTestFixture<TEndpoint> :
		EndpointTestFixture<TEndpoint>
		where TEndpoint : IEndpoint
	{
		[TestFixtureSetUp]
		public void LocalAndRemoteTestFixtureSetup()
		{
			SetupSubscriptionService();

			LocalBus = SetupServiceBus(LocalUri);
			RemoteBus = SetupServiceBus(RemoteUri);
		}

		private void SetupSubscriptionService()
		{
			SetupSubscriptionRepository();

			ObjectBuilder.SetupSagaRepository<SubscriptionClientSaga>();
			ObjectBuilder.SetupSagaRepository<SubscriptionSaga>();

			SubscriptionBus = SetupServiceBus(SubscriptionUri, x => { x.SetConcurrentConsumerLimit(1); });

			SubscriptionService = new SubscriptionService(SubscriptionBus,
				ObjectBuilder.GetInstance<ISubscriptionRepository>(),
				EndpointResolver,
				ObjectBuilder.GetInstance<ISagaRepository<SubscriptionSaga>>(),
				ObjectBuilder.GetInstance<ISagaRepository<SubscriptionClientSaga>>());

			SubscriptionService.Start();

			ObjectBuilder.Construct(() => new SubscriptionClient(EndpointResolver));
		}

		private void SetupSubscriptionRepository()
		{
			var subscriptionRepository = MockRepository.GenerateMock<ISubscriptionRepository>();
			subscriptionRepository.Stub(x => x.List()).Return(new List<Subscription>());
			ObjectBuilder.Add(subscriptionRepository);
		}

		[TestFixtureTearDown]
		public void LocalAndRemoteTestFixtureTeardown()
		{
			LocalBus = null;
			RemoteBus = null;
			SubscriptionService = null;
		}

		protected Uri LocalUri { get; set; }
		protected Uri RemoteUri { get; set; }
		protected Uri SubscriptionUri { get; set; }

		protected IServiceBus LocalBus { get; private set; }
		protected IServiceBus RemoteBus { get; private set; }
		protected IServiceBus SubscriptionBus { get; private set; }

		protected SubscriptionService SubscriptionService { get; private set; }

		protected override void ConfigureServiceBus(Uri uri, IServiceBusConfigurator configurator)
		{
			base.ConfigureServiceBus(uri, configurator);

			IControlBus controlBus = ControlBusConfigurator.New(x =>
				{
					x.ReceiveFrom(GetControlBusUri(uri));

					x.PurgeBeforeStarting();
				});

			configurator.ConfigureService<SubscriptionClientConfigurator>(y =>
				{
					// Subscription Endpoint
					y.SetSubscriptionServiceEndpoint(SubscriptionUri);
				});

			configurator.UseControlBus(controlBus);
		}

		protected Uri GetControlBusUri(Uri uri)
		{
			return uri.AppendToPath("_control");
		}
	}
}