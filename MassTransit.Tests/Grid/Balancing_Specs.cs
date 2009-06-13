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
namespace MassTransit.Tests.Grid
{
	using Magnum.DateTimeExtensions;
	using MassTransit.Grid.Configuration;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_multiple_nodes_support_the_same_services :
		GridTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			GridNodes.Each(x => x.ObjectBuilder.Stub(b => b.GetInstance<SimpleGridService>()).Return(new SimpleGridService()));

			WaitForServiceToBeAvailable<SimpleGridCommand>(5.Seconds(), 1);
		}

		protected override void ConfigureGridA(GridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();
		}

		protected override void ConfigureGridB(GridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();
		}

		protected override void ConfigureGridC(GridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();
		}

		[Test, Explicit]
		public void The_first_available_node_should_be_voted_on_by_the_participating_nodes()
		{


		}
	}
}