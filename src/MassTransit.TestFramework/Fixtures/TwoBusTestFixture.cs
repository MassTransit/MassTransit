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
namespace MassTransit.TestFramework.Fixtures
{
	using System;
	using BusConfigurators;
	using Exceptions;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Services.Subscriptions;
	using Subscriptions.Actors;

	[TestFixture]
	public class TwoBusTestFixture<TTransportFactory> :
		EndpointTestFixture<TTransportFactory>
		where TTransportFactory : ITransportFactory, new()
	{
		[TestFixtureSetUp]
		public void TwoBusTestFixtureSetup()
		{
			SetupSubscriptionService();

			LocalBus = SetupServiceBus(LocalUri);
			RemoteBus = SetupServiceBus(RemoteUri);
		}

		[TestFixtureTearDown]
		public void TwoBusTestFixtureTeardown()
		{
			LocalBus = null;
			RemoteBus = null;
			SubscriptionService = null;
		}

		protected Uri LocalUri { get; set; }
		protected Uri RemoteUri { get; set; }

		protected IServiceBus LocalBus { get; private set; }
		protected IServiceBus RemoteBus { get; private set; }
		protected ISubscriptionService SubscriptionService { get; private set; }

		protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
		{
			if (SubscriptionService == null)
				throw new ConfigurationException("The subscription service must be configured before creating a service bus");

			base.ConfigureServiceBus(uri, configurator);

			// really need a multicastLoopback transport for this to work properly with these changes
			configurator.AddService(BusServiceLayer.Session, () => new SubscriptionPublisher(SubscriptionService));
			configurator.AddService(BusServiceLayer.Session, () => new SubscriptionConsumer(SubscriptionService));
		}

		void SetupSubscriptionService()
		{
		}
	}
}