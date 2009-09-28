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
	using log4net;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit.Grid.Configuration;
	using MassTransit.Grid.Sagas;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_the_grid_services_are_attached_to_the_bus :
		GridTestFixture<LoopbackEndpoint>
	{
		[Test, Explicit]
		public void A_grid_service_framework_should_run_on_top_of_the_service_bus()
		{
			Thread.Sleep(500);

			Assert.AreEqual(3, nodeA.GridNodeRepository.Where(x => true).Count());
			Assert.AreEqual(3, nodeB.GridNodeRepository.Where(x => true).Count());
			Assert.AreEqual(3, nodeC.GridNodeRepository.Where(x => true).Count());
		}
	}

	[TestFixture]
	public class Configuring_a_service_to_run_on_the_grid :
		GridTestFixture<LoopbackEndpoint>
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			GridNodes.Each(x => x.ObjectBuilder.Stub(b => b.GetInstance<SimpleGridService>()).Return(new SimpleGridService()));
		}

		protected override void ConfigureGridA(IGridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();
		}

		[Test]
		public void Should_advertise_the_service_to_all_grid_nodes()
		{
			WaitForServiceToBeAvailable<SimpleGridCommand>(5.Seconds(), 1);
		}

		[Test, Explicit]
		public void Should_respond_when_commands_are_executed()
		{
			WaitForServiceToBeAvailable<SimpleGridCommand>(5.Seconds(), 1);

			var transactionId = CombGuid.Generate();
			var command = new SimpleGridCommand(transactionId);

			nodeB.DataBus.MakeRequest(x => nodeB.DataBus.Execute(command, context => context.SendResponseTo(nodeB.DataBus)))
				.When<SimpleGridResult>().RelatedTo(transactionId).IsReceived(result =>
					{
						Trace.WriteLine("Happy ending!");
					})
				.TimeoutAfter(2.Seconds())
				.OnTimeout(() => { throw new ApplicationException("Timeout waiting for response"); })
				.Send();
		}
	}

	[TestFixture]
	public class Configuring_a_service_to_run_on_multiple_nodes_of_the_grid :
		GridTestFixture<LoopbackEndpoint>
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			GridNodes.Each(x => x.ObjectBuilder.Stub(b => b.GetInstance<SimpleGridService>()).Return(new SimpleGridService()));
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
		}

		[Test]
		public void Multiple_nodes_advertising_should_only_have_one_instance_of_the_service()
		{
			WaitForServiceToBeAvailable<SimpleGridCommand>(5.Seconds(), 1);

			nodeC.GridServiceRepository.Where(x => true).Each(x => Trace.WriteLine(x.ServiceName + " = " + x.CorrelationId));

			nodeA.GridServiceRepository.Where(x => true).Count().ShouldEqual(1);
			nodeB.GridServiceRepository.Where(x => true).Count().ShouldEqual(1);
			nodeC.GridServiceRepository.Where(x => true).Count().ShouldEqual(1);

			var serviceId = GridService.GenerateIdForType(typeof (SimpleGridCommand));

			nodeA.GridServiceNodeRepository.Where(x => x.ServiceId == serviceId).Count().ShouldEqual(2);
			nodeB.GridServiceNodeRepository.Where(x => x.ServiceId == serviceId).Count().ShouldEqual(2);
			nodeC.GridServiceNodeRepository.Where(x => x.ServiceId == serviceId).Count().ShouldEqual(2);

			GridNodes.Each(node => node.GridServiceNodeRepository
			                       	.Where(x => x.ServiceId == serviceId)
			                       	.Each(x => Trace.WriteLine(node.ControlBus.Endpoint.Uri + " => " + x.DataUri)));
		}
	}

	public class SimpleGridService :
		Consumes<SimpleGridCommand>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SimpleGridService));

		public int WorkerLimit { get; set; }

		public void Consume(SimpleGridCommand message)
		{
			_log.InfoFormat("{0} -DONE-: {1}", CurrentMessage.Headers.Bus.Endpoint.Uri,message.CorrelationId);

			Thread.Sleep(10);
			CurrentMessage.Respond(new SimpleGridResult(message.CorrelationId));
		}
	}

	[Serializable]
	public class SimpleGridBase :
		CorrelatedBy<Guid>
	{
		public SimpleGridBase(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected SimpleGridBase()
		{
		}

		public Guid CorrelationId { get; set; }
	}

	[Serializable]
	public class SimpleGridCommand :
		SimpleGridBase
	{
		public SimpleGridCommand(Guid correlationId)
			: base(correlationId)
		{
		}

		protected SimpleGridCommand()
		{
		}
	}

	[Serializable]
	public class SimpleGridResult :
		SimpleGridBase
	{
		public SimpleGridResult(Guid correlationId)
			: base(correlationId)
		{
			CreatedAt = DateTime.UtcNow;
		}

		protected SimpleGridResult()
		{
		}

		public DateTime CreatedAt { get; set; }
	}
}