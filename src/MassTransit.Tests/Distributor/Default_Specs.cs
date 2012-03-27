// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Distributor
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using BusConfigurators;
	using Load;
	using Load.Messages;
	using Magnum.Extensions;
	using MassTransit.Distributor;
	using MassTransit.Distributor.Messages;
	using MassTransit.Pipeline.Inspectors;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestFramework;
	using Util;

	[TestFixture]
	public class Default_distributor_specifications :
		LoopbackDistributorTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			AddFirstCommandInstance("A", "loopback://localhost/a");
			AddFirstCommandInstance("B", "loopback://localhost/b");
			AddFirstCommandInstance("C", "loopback://localhost/c");

			RemoteBus.ShouldHaveRemoteSubscriptionFor<Distributed<FirstCommand>>();
		}

		[Test]
		public void Can_collect_iworkeravaiable_messages()
		{
			int workerAvaiableCountRecieved = 0;
			var messageRecieved = new ManualResetEvent(false);

			UnsubscribeAction unsubscribe = LocalBus.SubscribeHandler<IWorkerAvailable>(message =>
				{
					Interlocked.Increment(ref workerAvaiableCountRecieved);
					messageRecieved.Set();
				});

			Instances.ToList().ForEach(x => { x.Value.DataBus.ControlBus.Endpoint.Send(new PingWorker()); });

			messageRecieved.WaitOne(3.Seconds());

			unsubscribe();

			workerAvaiableCountRecieved.ShouldBeGreaterThan(0);
		}

		[Test, Explicit]
		public void Ensure_distributor_sends_ping_request_after_timeout()
		{
			int pingRequestsRecieved = 0;
			var messageRecieved = new ManualResetEvent(false);

			UnsubscribeAction unsubscribe = Instances["A"].DataBus.ControlBus.SubscribeHandler<PingWorker>(message =>
				{
					Interlocked.Increment(ref pingRequestsRecieved);
					messageRecieved.Set();
				});

			messageRecieved.WaitOne(120.Seconds());

			unsubscribe();

			pingRequestsRecieved.ShouldBeGreaterThan(0);
		}

		[Test]
		public void Ensure_workers_will_respond_to_ping_request()
		{
			int workerAvaiableCountRecieved = 0;
			var messageRecieved = new ManualResetEvent(false);

			UnsubscribeAction unsubscribe = LocalBus.SubscribeHandler<WorkerAvailable<FirstCommand>>(message =>
				{
					Interlocked.Increment(ref workerAvaiableCountRecieved);
					messageRecieved.Set();
				});

			Instances.ToList().ForEach(x => { x.Value.DataBus.ControlBus.Endpoint.Send(new PingWorker()); });

			messageRecieved.WaitOne(3.Seconds());

			unsubscribe();

			workerAvaiableCountRecieved.ShouldBeGreaterThan(0);
		}

		[Test]
		public void The_pipeline_viewer_should_show_the_distributor()
		{
			PipelineViewer.Trace(LocalBus.InboundPipeline);

			PipelineViewer.Trace(Instances["A"].DataBus.InboundPipeline);
		}

		[Test, Explicit]
		public void Using_the_load_generator_should_share_the_load()
		{
			var generator = new LoadGenerator<FirstCommand, FirstResponse>();
			const int count = 100;

			generator.Run(RemoteBus, LocalBus.Endpoint, Instances.Values.Select(x => x.DataBus), count, x => new FirstCommand(x));

			Dictionary<Uri, int> results = generator.GetWorkerLoad();

			Assert.That(results.Sum(x => x.Value), Is.EqualTo(count));
			results.ToList().ForEach(x =>
			                         Assert.That(x.Value, Is.GreaterThan(0).And.LessThanOrEqualTo(count),
			                         	string.Format("{0} did not consume between 0 and {1}",
			                         		x.Key.ToString(), count)));
		}
	}

	[TestFixture]
	public class Distributor_with_custom_worker_selection_strategy :
		LoopbackDistributorTestFixture
	{
		Dictionary<String, Uri> _nodes = new Dictionary<string, Uri>
			{
				{"A", new Uri("loopback://localhost/a")},
				{"B", new Uri("loopback://localhost/b")},
				{"C", new Uri("loopback://localhost/c")}
			};

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_nodes.ToList().ForEach(x => AddFirstCommandInstance(x.Key, x.Value.ToString()));
		}

		protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
		{
			var mock = MockRepository.GenerateStub<IWorkerSelectionStrategy<FirstCommand>>();
			mock.Stub(x => x.SelectWorker(null, null))
				.IgnoreArguments()
				.Return(new WorkerDetails
					{
						ControlUri = _nodes["A"].AppendToPath("_control"),
						DataUri = _nodes["A"],
						InProgress = 0,
						InProgressLimit = 100,
						LastUpdate = DateTime.UtcNow
					}
				);

			mock.Stub(x => x.HasAvailableWorker(null, null))
				.IgnoreArguments()
				.Return(true);

			configurator.UseDistributorFor(mock);
		}

		[Test]
		public void Node_a_should_recieve_all_the_work()
		{
			var generator = new LoadGenerator<FirstCommand, FirstResponse>();
			const int count = 100;

			generator.Run(RemoteBus, LocalBus.Endpoint, Instances.Values.Select(x => x.DataBus), count, x => new FirstCommand(x));

			Dictionary<Uri, int> results = generator.GetWorkerLoad();

			Assert.That(results.Sum(x => x.Value), Is.EqualTo(count));
			Assert.That(results[_nodes["A"]], Is.EqualTo(count));
		}
	}
}