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
namespace MassTransit.Transports.Msmq.Tests.TestFixtures
{
	using Configuration;
	using MassTransit.Tests.TextFixtures;

	public class MsmqTransactionalEndpointTestFixture :
		EndpointTestFixture<MsmqEndpoint>
	{
		private const string _localEndpointUri = "msmq://localhost/mt_client_tx";
		private const string _localErrorUri = "msmq://localhost/mt_client_error_tx";
		private const string _remoteEndpointUri = "msmq://localhost/mt_server_tx";

		private MsmqEndpoint _localEndpoint = new MsmqEndpoint(_localEndpointUri);
		private MsmqEndpoint _localErrorEndpoint = new MsmqEndpoint(_localErrorUri);
		private MsmqEndpoint _remoteEndpoint = new MsmqEndpoint(_remoteEndpointUri);

		public string LocalEndpointUri
		{
			get { return _localEndpointUri; }
		}

		public string LocalErrorUri
		{
			get { return _localErrorUri; }
		}

		public string RemoteEndpointUri
		{
			get { return _remoteEndpointUri; }
		}

		public MsmqEndpoint LocalEndpoint
		{
			get { return _localEndpoint; }
		}

		public MsmqEndpoint LocalErrorEndpoint
		{
			get { return _localErrorEndpoint; }
		}

		public MsmqEndpoint RemoteEndpoint
		{
			get { return _remoteEndpoint; }
		}

		public IServiceBus LocalBus { get; private set; }
		public IServiceBus RemoteBus { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			LocalBus = ServiceBusConfigurator.New(x =>
				{
					x.ReceiveFrom(LocalEndpointUri);
					x.SendErrorsTo(LocalErrorUri);
				});

			RemoteBus = ServiceBusConfigurator.New(x => { x.ReceiveFrom(RemoteEndpointUri); });
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