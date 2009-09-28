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
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using MassTransit.Grid.Configuration;
	using MassTransit.Grid.Sagas;
	using NUnit.Framework;
	using TextFixtures;

	[TestFixture]
	public class GridTestFixture<TEndpoint> :
		SubscriptionServiceTestFixture<TEndpoint>
		where TEndpoint:IEndpoint
	{
		public string BaseGridNodeUri = "loopback://localhost/mt_grid_";

		public TestGridNode nodeA { get; private set; }
		public TestGridNode nodeB { get; private set; }
		public TestGridNode nodeC { get; private set; }

		public TestGridNode[] GridNodes
		{
			get { return new[] {nodeA, nodeB, nodeC}; }
		}

		protected override void EstablishContext()
		{
			base.EstablishContext();

			nodeA = new TestGridNode(BaseGridNodeUri+"a", EndpointFactory, SubscriptionServiceUri, ConfigureGridA);
			nodeB = new TestGridNode(BaseGridNodeUri+"b", EndpointFactory, SubscriptionServiceUri, ConfigureGridB);
			nodeC = new TestGridNode(BaseGridNodeUri+"c", EndpointFactory, SubscriptionServiceUri, ConfigureGridC);

			Thread.Sleep(500);
		}

		protected virtual void ConfigureGridA(IGridConfigurator grid)
		{
		}

		protected virtual void ConfigureGridB(IGridConfigurator grid)
		{
		}

		protected virtual void ConfigureGridC(IGridConfigurator grid)
		{
		}

		protected void WaitForServiceToBeAvailable<TService>(TimeSpan timeout, int nodeCount)
		{
			Guid serviceId = GridService.GenerateIdForType(typeof (TService));

			DateTime giveUpAt = DateTime.Now + timeout;
			ManualResetEvent neverSurrender = new ManualResetEvent(false);

			int expectedTotal = 3*nodeCount;

			while ( DateTime.Now < giveUpAt )
			{
				int total = 0;
				foreach (var node in GridNodes)
				{
					int count = node.GridServiceRepository.Where(y => y.CorrelationId == serviceId).Count();
					total += count;
				}

				if (total == expectedTotal)
					return;

				Trace.WriteLine("Only " + total + ", waiting for " + expectedTotal);

				neverSurrender.WaitOne(30, true);
			}

			Assert.Fail("Timeout waiting for " + typeof (TService).Name);
		}

		private void TeardownGridNodes()
		{
			nodeA.Dispose();
			nodeA = null;
			nodeB.Dispose();
			nodeB = null;
			nodeC.Dispose();
			nodeC = null;
		}

		protected override void TeardownContext()
		{
			TeardownGridNodes();

			base.TeardownContext();
		}
	}
}