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
namespace WebLoadTest
{
	using MassTransit.StructureMapIntegration;
	using MassTransit.Transports.Msmq;

	public class WebLoadTestRegistry :
		MassTransitRegistryBase
	{
		public WebLoadTestRegistry()
		{
			MsmqEndpointConfigurator.Defaults(x =>
				{
					x.CreateMissingQueues = true;
					x.CreateTransactionalQueues = true;
					x.PurgeOnStartup = true;
				});

			RegisterEndpointFactory(x =>
				{
					x.RegisterTransport<MsmqEndpoint>();
				});

			RegisterServiceBus("msmq://localhost/web_load_test", x => { });
		}
	}
}