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
namespace MassTransit.Transports.Msmq.Tests
{
	using Grid.Configuration;
	using Magnum.DateTimeExtensions;
	using MassTransit.Tests.Grid;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture, Category("Integration")]
	public class ThreeNodeMsmqGridTestFixture :
		GridTestFixture<MsmqEndpoint>
	{
		protected override void EstablishContext()
		{
			BaseGridNodeUri = BaseGridNodeUri.Replace("loopback", "msmq");
			SubscriptionServiceUri = SubscriptionServiceUri.Replace("loopback", "msmq");
			ClientControlUri = ClientControlUri.Replace("loopback", "msmq");
			ServerControlUri = ServerControlUri.Replace("loopback", "msmq");
			ClientUri = ClientUri.Replace("loopback", "msmq");
			ServerUri = ServerUri.Replace("loopback", "msmq");

			base.EstablishContext();

			GridNodes.Each(x => x.ObjectBuilder.Stub(b => b.GetInstance<SimpleGridService>()).Return(new SimpleGridService()));

			WaitForServiceToBeAvailable<SimpleGridCommand>(15.Seconds(), 1);
		}

		protected override void AdditionalEndpointFactoryConfiguration(Configuration.IEndpointFactoryConfigurator x)
		{
			base.AdditionalEndpointFactoryConfiguration(x);

			MsmqEndpointConfigurator.Defaults(y =>
				{
					y.CreateMissingQueues = true;
					y.CreateTransactionalQueues = false;
					y.PurgeOnStartup = true;
				});
		}

		protected override void ConfigureGridA(IGridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();
		}

		protected override void ConfigureGridB(IGridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();

			grid.SetProposer();
		}

		protected override void ConfigureGridC(IGridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();
		}
	}
}