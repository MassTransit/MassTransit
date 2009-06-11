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
	using System.Threading;
	using NUnit.Framework;
	using TextFixtures;

	[TestFixture]
	public class GridTestFixture :
		SubscriptionServiceTestFixture
	{
		public TestGridNode nodeA { get; private set; }
		public TestGridNode nodeB { get; private set; }
		public TestGridNode nodeC { get; private set; }

		public TestGridNode[] GridNodes
		{
			get { return new[] { nodeA, nodeB, nodeC }; }
		}

		protected override void EstablishContext()
		{
			base.EstablishContext();

			nodeA = new TestGridNode("a", EndpointFactory, SubscriptionServiceEndpointAddress);
			nodeB = new TestGridNode("b", EndpointFactory, SubscriptionServiceEndpointAddress);
			nodeC = new TestGridNode("c", EndpointFactory, SubscriptionServiceEndpointAddress);

			Thread.Sleep(500);
		}

		private void TeardownGrideNode(ref ServiceGrid grid, ref IControlBus controlBus, ref IServiceBus dataBus)
		{
			nodeA.Dispose();
			nodeA = null;
			nodeB.Dispose();
			nodeB = null;
			nodeC.Dispose();
			nodeC = null;

			dataBus.Dispose();
			dataBus = null;

			controlBus.Dispose();
			controlBus = null;
		}

		protected override void TeardownContext()
		{
			nodeC.Dispose();
			nodeC = null;

			base.TeardownContext();
		}
	}
}