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
	using System.Threading;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit.Grid;
	using MassTransit.Grid.Configuration;
	using MassTransit.Grid.Paxos;
	using MassTransit.Grid.Sagas;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class Using_the_load_balancer_with_the_grid :
		GridTestFixture<LoopbackEndpoint>
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			GridNodes.Each(x => x.ObjectBuilder.Stub(b => b.GetInstance<SimpleGridService>()).Return(new SimpleGridService()));
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


		[Test]
		public void Should_promote_the_least_busy_node_to_the_preferred_node()
		{
			WaitForServiceToBeAvailable<SimpleGridCommand>(5.Seconds(), 1);

			GridServiceLoadBalancer balancerA = new GridServiceLoadBalancer(nodeA.GridAcceptorRepository, nodeA.GridListenerRepository);
			balancerA.Start(nodeA.DataBus);
			GridServiceLoadBalancer balancerB = new GridServiceLoadBalancer(nodeB.GridAcceptorRepository, nodeB.GridListenerRepository);
			balancerB.Start(nodeB.DataBus);
			GridServiceLoadBalancer balancerC = new GridServiceLoadBalancer(nodeC.GridAcceptorRepository, nodeC.GridListenerRepository);
			balancerC.Start(nodeC.DataBus);

			ManualResetEvent enoughPromises = new ManualResetEvent(false);
			int promiseCount = 0;
			var unsubscribe = LocalBus.Subscribe<Promise<AvailableGridServiceNode>>(message =>
				{
					Trace.WriteLine("Promise received from " + CurrentMessage.Headers.SourceAddress);
					promiseCount++;
					if (promiseCount >= 2)
						enoughPromises.Set();
				});

			ManualResetEvent enoughAcceptors = new ManualResetEvent(false);
			int acceptorCount = 0;
			unsubscribe += LocalBus.Subscribe<Accepted<AvailableGridServiceNode>>(message =>
				{
					Trace.WriteLine("Accepted received from " + CurrentMessage.Headers.SourceAddress);
					acceptorCount++;
					if (acceptorCount >= 2)
						enoughAcceptors.Set();
				});

			Thread.Sleep(1500);

			Guid serviceId = GridService.GenerateIdForType(typeof (SimpleGridCommand));
			Guid leaderId = CombGuid.Generate();

			LocalBus.Publish(new Prepare<AvailableGridServiceNode>
				{
					BallotId = 1,
					CorrelationId = serviceId,
					LeaderId = leaderId,
				});

			Assert.IsTrue(enoughPromises.WaitOne(5.Seconds(), true), "Not enough promises");

			LocalBus.Publish(new Accept<AvailableGridServiceNode>
				{
					BallotId = 1,
					CorrelationId = serviceId,
					LeaderId = leaderId,
					Value= new AvailableGridServiceNode() {ControlUri = nodeA.ControlBus.Endpoint.Uri, DataUri = nodeA.DataBus.Endpoint.Uri },
				});

			Assert.IsTrue(enoughAcceptors.WaitOne(5.Seconds(), true), "Not enough acceptors");

			unsubscribe();
		}
	}
}