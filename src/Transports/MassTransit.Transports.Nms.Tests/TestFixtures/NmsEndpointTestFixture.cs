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
namespace MassTransit.Transports.Nms.Tests.TestFixtures
{
	using System;
	using MassTransit.Tests.TextFixtures;
	using Services.Subscriptions;

	public class NmsEndpointTestFixture :
		EndpointTestFixture<NmsTransportFactory>
	{
		Uri _localEndpointUri;
		Uri _remoteEndpointUri;
		protected ISubscriptionService SubscriptionService { get; set; }
		protected IServiceBus LocalBus { get; set; }
		protected IServiceBus RemoteBus { get; set; }

		protected string ActiveMQHostName { get; set; }

		protected NmsEndpointTestFixture()
		{
			ActiveMQHostName = "192.168.0.195";
		}

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_localEndpointUri = new UriBuilder("activemq", ActiveMQHostName, 61616, "mt_client").Uri;
			_remoteEndpointUri = new UriBuilder("activemq", ActiveMQHostName, 61616, "mt_server").Uri;

			SetupSubscriptionService();

			LocalBus = ServiceBusFactory.New(x =>
				{
					ConnectSubscriptionService(x, SubscriptionService);
					x.ReceiveFrom(_localEndpointUri);
				});

			RemoteBus = ServiceBusFactory.New(x =>
				{
					ConnectSubscriptionService(x, SubscriptionService);
					x.ReceiveFrom(_remoteEndpointUri);
				});
		}

		void SetupSubscriptionService()
		{
			SubscriptionService = new LocalSubscriptionService();
		}

		protected override void TeardownContext()
		{
			LocalBus.Dispose();
			LocalBus = null;

			RemoteBus.Dispose();
			RemoteBus = null;

			SubscriptionService = null;

			base.TeardownContext();
		}
	}
}