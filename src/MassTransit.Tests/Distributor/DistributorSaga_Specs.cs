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
namespace MassTransit.Tests.Distributor
{
	using Load;
	using Load.Messages;
	using Load.Sagas;
	using MassTransit.Distributor.Messages;
	using MassTransit.Pipeline.Inspectors;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Using_the_distributor_for_a_saga :
		LoopbackDistributorSagaTestFixture
	{
		[Test]
		public void Should_have_a_subscription_for_the_first_command()
		{
			LocalBus.ShouldHaveSubscriptionFor<FirstCommand>();
		}

		[Test]
		public void Should_have_a_subscription_for_the_pending_command()
		{
			LocalBus.ShouldHaveSubscriptionFor<FirstPending>();
		}
	}

	[TestFixture]
	public class Using_the_distributor_saga_worker_for_a_saga :
		LoopbackDistributorSagaTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			ServiceInstance instance = AddInstance("A", "loopback://localhost/a", x =>
				{
					//x.ImplementDistributorSagaWorker<FirstSaga>(FirstSagaRepository);
				});

			instance.ObjectBuilder.Add(FirstSagaRepository);
			instance.DataBus.Subscribe<FirstSaga>();

			//AddFirstCommandInstance("B", "loopback://localhost/b");
			//AddFirstCommandInstance("C", "loopback://localhost/c");
		}

		[Test, Explicit]
		public void Should_register_the_message_consumers()
		{
			Instances["A"].DataBus.ShouldHaveSubscriptionFor<Distributed<FirstCommand>>();
		}

		[Test]
		public void Using_the_load_generator_should_share_the_load()
		{
			var generator = new LoadGenerator<FirstCommand, FirstResponse>();

			generator.Run(RemoteBus, 100, x => new FirstCommand(x));
		}

		[Test]
		public void The_pipeline_viewer_should_show_the_distributor()
		{
			PipelineViewer.Trace(Instances["A"].DataBus.InboundPipeline);
		}
	}
}