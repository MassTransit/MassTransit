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
namespace MassTransit.Transports.Msmq.Tests.TestFixtures
{
	using System;
	using MassTransit.Tests.TextFixtures;
	using Services.Subscriptions;

	public class MsmqEndpointTestFixture :
		EndpointTestFixture<MsmqTransportFactory>
	{
		protected Uri LocalEndpointUri { get; set; }
		protected Uri LocalErrorUri { get; set; }
		protected Uri RemoteEndpointUri { get; set; }

		ISubscriptionService SubscriptionService { get; set; }

		protected IServiceBus LocalBus { get; set; }
		protected IServiceBus RemoteBus { get; set; }

		public MsmqEndpointTestFixture()
			: this(new EndpointSettings("msmq://localhost/mt_client"))
		{
		}

		public MsmqEndpointTestFixture(EndpointSettings settings)
		{
			LocalEndpointUri = settings.Address.Uri;
			LocalErrorUri = settings.ErrorAddress.Uri;
			RemoteEndpointUri = new Uri("msmq://localhost/mt_server");

			ConfigureEndpointFactory(x =>
				{
					x.SetCreateMissingQueues(true);
					x.SetCreateTransactionalQueues(settings.Transactional);
					x.SetPurgeOnStartup(true);
				});
		}

		protected override void EstablishContext()
		{
			base.EstablishContext();

			LocalEndpoint = EndpointCache.GetEndpoint(LocalEndpointUri);
			LocalErrorEndpoint = EndpointCache.GetEndpoint(LocalErrorUri);
			RemoteEndpoint = EndpointCache.GetEndpoint(RemoteEndpointUri);

			SetupSubscriptionService();

			LocalBus = ServiceBusFactory.New(x =>
				{
					ConnectSubscriptionService(x, SubscriptionService);
					x.ReceiveFrom(LocalEndpointUri);
				});

			RemoteBus = ServiceBusFactory.New(x =>
				{
					ConnectSubscriptionService(x, SubscriptionService);
					x.ReceiveFrom(RemoteEndpointUri);
				});
		}

		protected void Purge(IEndpointAddress address)
		{
			IEndpointManagement management = MsmqEndpointManagement.New(address.Uri);
			management.Purge();
		}

		void SetupSubscriptionService()
		{
			SubscriptionService = new LocalSubscriptionService();
		}


		protected IEndpoint LocalEndpoint { get; set; }
		protected IEndpoint LocalErrorEndpoint { get; set; }
		protected IEndpoint RemoteEndpoint { get; set; }

		protected override void TeardownContext()
		{
			LocalBus.Dispose();
			LocalBus = null;

			RemoteBus.Dispose();
			RemoteBus = null;

			LocalEndpoint = null;
			LocalErrorEndpoint = null;
			RemoteEndpoint = null;

			base.TeardownContext();
		}
	}
}