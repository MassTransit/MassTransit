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
	using MassTransit.Pipeline.Inspectors;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class Default_distributor_specifications :
		LoopbackDistributorTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			AddInstance("A", "loopback://localhost/a");
			AddInstance("B", "loopback://localhost/b");
			AddInstance("C", "loopback://localhost/c");
		}

		[Test]
		public void Using_the_load_generator_should_share_the_load()
		{
			var generator = new LoadGenerator<FirstCommand, FirstResponse>();

			generator.Run(RemoteBus, 100);
		}

		[Test]
		public void The_pipeline_viewer_should_show_the_distributor()
		{
			PipelineViewer.Trace(LocalBus.InboundPipeline);

			PipelineViewer.Trace(Instances["A"].DataBus.InboundPipeline);
		}
	}
}