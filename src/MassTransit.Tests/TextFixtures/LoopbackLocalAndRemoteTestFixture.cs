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
	using BusConfigurators;
	using MassTransit.Services.Subscriptions;
	using MassTransit.Transports;
	using MassTransit.Transports.Loopback;
	using NUnit.Framework;

	[TestFixture]
	public class LoopbackLocalAndRemoteTestFixture :
		EndpointTestFixture<LoopbackTransportFactory>
	{
		public ISubscriptionService SubscriptionService { get; private set; }
		public IServiceBus LocalBus { get; private set; }
		public IServiceBus RemoteBus { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			SetupSubscriptionService();

			LocalBus = ServiceBusFactory.New(x =>
				{
					ConnectSubscriptionService(x, SubscriptionService);
					x.ReceiveFrom("loopback://localhost/mt_client");

					ConfigureLocalBus(x);
				});

			RemoteBus = ServiceBusFactory.New(x =>
				{
					ConnectSubscriptionService(x, SubscriptionService);
					x.ReceiveFrom("loopback://localhost/mt_server");

					ConfigureRemoteBus(x);
				});
		}

		protected virtual void ConfigureLocalBus(ServiceBusConfigurator configurator)
		{
		}

		protected virtual void ConfigureRemoteBus(ServiceBusConfigurator configurator)
		{
		}

		void SetupSubscriptionService()
		{
			SubscriptionService = new LocalSubscriptionService();
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