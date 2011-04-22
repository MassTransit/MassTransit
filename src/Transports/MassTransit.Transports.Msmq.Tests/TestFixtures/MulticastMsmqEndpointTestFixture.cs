// Copyright 2007-2011 The Apache Software Foundation.
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
	using Configuration;
	using Internal;
	using MassTransit.Tests.TextFixtures;
	using Services.Subscriptions;

	public class MulticastMsmqEndpointTestFixture :
		EndpointTestFixture<MsmqTransportFactory>
	{
		protected Uri LocalEndpointUri { get; set; }
		protected Uri LocalErrorUri { get; set; }
		protected Uri RemoteEndpointUri { get; set; }

		private ISubscriptionService SubscriptionService { get; set; }

		protected IServiceBus LocalBus { get; set; }
		protected IServiceBus RemoteBus { get; set; }

		public MulticastMsmqEndpointTestFixture()
		{
			EndpointConfigurator.Defaults(x =>
				{
					x.CreateMissingQueues = true;
					x.CreateTransactionalQueues = false;
					x.PurgeOnStartup = true;
				});

			AddTransport<MulticastMsmqTransportFactory>();

			LocalEndpointUri = new Uri("msmq://localhost/mt_client");
			LocalErrorUri = new Uri("msmq://localhost/mt_client_error");
			RemoteEndpointUri = new Uri("msmq://localhost/mt_server");
		}

		protected override void EstablishContext()
		{
			base.EstablishContext();

			LocalEndpoint = EndpointResolver.GetEndpoint(LocalEndpointUri);
			LocalErrorEndpoint = EndpointResolver.GetEndpoint(LocalErrorUri);
			RemoteEndpoint = EndpointResolver.GetEndpoint(RemoteEndpointUri);

			LocalBus = ServiceBusConfigurator.New(x =>
				{
					x.UseMulticastSubscriptionClient();
					x.ReceiveFrom(LocalEndpointUri);
					x.SendErrorsTo(LocalErrorUri);
				});

			RemoteBus = ServiceBusConfigurator.New(x =>
				{
					x.UseMulticastSubscriptionClient();
					x.ReceiveFrom(RemoteEndpointUri);
				});
		}

		protected void Purge(IEndpointAddress address)
		{
			IEndpointManagement management = MsmqEndpointManagement.New(address.Uri);
			management.Purge();
		}

		public IEndpoint LocalEndpoint { get; private set; }
		public IEndpoint LocalErrorEndpoint { get; private set; }
		public IEndpoint RemoteEndpoint { get; private set; }

		protected override void TeardownContext()
		{
			LocalBus.Dispose();
			LocalBus = null;

			RemoteBus.Dispose();
			RemoteBus = null;

			LocalEndpoint = null;
			LocalErrorEndpoint = null;

			base.TeardownContext();
		}
	}
}