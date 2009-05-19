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
	using System;
	using NUnit.Framework;
	using TextFixtures;

	[TestFixture]
	public class GridService_Specs :
		SubscriptionServiceTestFixture
	{
		[Test]
		public void A_grid_service_framework_should_run_on_top_of_the_service_bus()
		{
			Grid grid = new ServiceGrid(EndpointFactory);

			grid.Start(LocalBus);

			grid.Execute(new SimpleGridCommand());



			grid.ConfigureService<MyService>(x =>
				{
					x.WorkerLimit = 8;
				});




		}
	}

	public class MyService
	{
		public int WorkerLimit { get; set; }
	}

	public class SimpleCommandService
	{
	}

	[Serializable]
	public class SimpleGridCommand
	{
	}
}