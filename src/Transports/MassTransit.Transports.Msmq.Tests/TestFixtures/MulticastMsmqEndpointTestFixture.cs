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
	using BusConfigurators;
	using MassTransit.Tests.TextFixtures;

	public class MulticastMsmqEndpointTestFixture :
		EndpointTestFixture<MsmqTransportFactory>
	{
		protected Uri LocalEndpointUri { get; set; }
		protected Uri LocalErrorUri { get; set; }
		protected Uri RemoteEndpointUri { get; set; }

		protected IServiceBus LocalBus { get; set; }
		protected IServiceBus RemoteBus { get; set; }

		public MulticastMsmqEndpointTestFixture()
		{
			AddTransport<MulticastMsmqTransportFactory>();

			LocalEndpointUri = new Uri("msmq://localhost/mt_client");
			LocalErrorUri = new Uri("msmq://localhost/mt_client_error");
			RemoteEndpointUri = new Uri("msmq://localhost/mt_server");
		}

		protected override void EstablishContext()
		{
			base.EstablishContext();

			LocalBus = ServiceBusFactory.New(ConfigureLocalBus);

			RemoteBus = ServiceBusFactory.New(ConfigureRemoteBus);
		}

		protected virtual void ConfigureLocalBus(ServiceBusConfigurator configurator)
		{
			configurator.ReceiveFrom(LocalEndpointUri);
			configurator.UseMsmq(x => x.UseMulticastSubscriptionClient());
		}

		protected virtual void ConfigureRemoteBus(ServiceBusConfigurator configurator)
		{
			configurator.ReceiveFrom(RemoteEndpointUri);
            configurator.UseMsmq(x => x.UseMulticastSubscriptionClient());
        }

		protected override void TeardownContext()
		{
			LocalBus.Dispose();
			LocalBus = null;

			RemoteBus.Dispose();
			RemoteBus = null;

			base.TeardownContext();
		}
	}
}