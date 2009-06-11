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
	using Magnum;
	using Magnum.DateTimeExtensions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class GridService_Specs :
		GridTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			GridNodes.Each(x => x.ObjectBuilder.Stub(b => b.GetInstance<SimpleGridService>()).Return(new SimpleGridService()));
		}

		[Test, Explicit]
		public void A_grid_service_framework_should_run_on_top_of_the_service_bus()
		{
			Thread.Sleep(500);

			Assert.AreEqual(3, nodeA.NodeStateRepository.Where(x => true).Count());
			Assert.AreEqual(3, nodeB.NodeStateRepository.Where(x => true).Count());
			Assert.AreEqual(3, nodeC.NodeStateRepository.Where(x => true).Count());

//			grid.Execute(new SimpleGridCommand());
//
//
//
//			grid.ConfigureService<MyService>(x =>
//				{
//					x.WorkerLimit = 8;
//				});




		}

		[Test, Explicit]
		public void A_service_should_be_registered_to_the_grid()
		{
			nodeA.ServiceGrid.ConfigureService<SimpleGridService>(x =>
				{
				});

			Thread.Sleep(200);
			var transactionId = CombGuid.Generate();
			nodeB.DataBus.MakeRequest(x => x.Publish(new SimpleGridCommand(transactionId), context => context.SendResponseTo(nodeB.DataBus)))
				.When<SimpleGridResult>().RelatedTo(transactionId).IsReceived(result =>
					{
						Trace.WriteLine("Happy ending!");
					})
				.TimeoutAfter(2.Seconds())
				.OnTimeout(() => { throw new ApplicationException("Timeout waiting for response"); })
				.Send();
		}
	}

	public class SimpleGridService :
		Consumes<SimpleGridCommand>.All
	{
		public int WorkerLimit { get; set; }

		public void Consume(SimpleGridCommand message)
		{
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
		}
		protected SimpleGridResult()
		{
		}

	}
}