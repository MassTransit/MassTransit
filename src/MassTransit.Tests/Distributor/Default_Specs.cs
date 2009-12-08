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
    using System;
    using System.Linq;
    using System.Collections.Generic;
	using Load;
	using Load.Messages;
	using MassTransit.Pipeline.Inspectors;
    using Configuration;
    using MassTransit.Distributor;
    using Rhino.Mocks;
	using NUnit.Framework;

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
		}

		[Test]
		public void Using_the_load_generator_should_share_the_load()
		{
			var generator = new LoadGenerator<FirstCommand, FirstResponse>();
		    const int count = 100;

			generator.Run(RemoteBus, count, x => new FirstCommand(x));

		    var results = generator.GetWorkerLoad();

            Assert.That(results.Sum(x => x.Value), Is.EqualTo(count));
		}

		[Test]
		public void The_pipeline_viewer_should_show_the_distributor()
		{
			PipelineViewer.Trace(LocalBus.InboundPipeline);

			PipelineViewer.Trace(Instances["A"].DataBus.InboundPipeline);
		}
	}

    [TestFixture]
    public class Distributor_with_custom_worker_selection_strategy :
        LoopbackDistributorTestFixture
    {
        private Dictionary<String, Uri> _nodes = new Dictionary<string, Uri>()
            {
               { "A", new Uri("loopback://localhost/a") },
               { "B", new Uri("loopback://localhost/b") },
               { "C", new Uri("loopback://localhost/c") }
            };

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _nodes.ToList().ForEach(x => AddFirstCommandInstance(x.Key, x.Value.ToString()));
        }

        protected override void ConfigureLocalBus(IServiceBusConfigurator configurator)
        {
            var mock = MockRepository.GenerateStub<IWorkerSelectionStrategy<FirstCommand>>();
            mock.Stub(x => x.GetAvailableWorkers(null, null))
                .IgnoreArguments()
                .Return(new List<WorkerDetails>() 
                { 
                    new WorkerDetails() 
                    { 
                        ControlUri = _nodes["A"].AppendToPath("_control"), 
                        DataUri = _nodes["A"],
                        InProgress = 0,
                        InProgressLimit = 100,
                        LastUpdate = DateTime.UtcNow
                    } 
                });

            configurator.UseDistributorFor(EndpointFactory, mock);
        }

        [Test]
        public void Node_a_should_recieve_all_the_work()
        {
            var generator = new LoadGenerator<FirstCommand, FirstResponse>();
            const int count = 100;

            generator.Run(RemoteBus, count, x => new FirstCommand(x));

            var results = generator.GetWorkerLoad();

            Assert.That(results.Sum(x => x.Value), Is.EqualTo(count));
            Assert.That(results[_nodes["A"]], Is.EqualTo(count));
        }
    }
}