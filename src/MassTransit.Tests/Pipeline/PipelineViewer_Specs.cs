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
namespace MassTransit.Tests.Pipeline
{
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Inspectors;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class When_working_with_an_existing_pipeline
	{
		[SetUp]
		public void Setup()
		{
            _pipeline = InboundPipelineConfigurator.CreateDefault(null);
		}

		private IInboundMessagePipeline _pipeline;

		[Test]
		public void I_want_to_display_the_entire_flow_through_the_pipeline()
		{
			PipelineViewer.Trace(_pipeline);
		}

	    [Test]
	    public void I_want_to_display_a_more_detailed_flow()
	    {
           // _pipeline.Filter<object>(m => true);
            _pipeline.ConnectHandler<PingMessage>(m => { }, x => { return true; });
	        
            PipelineViewer.Trace(_pipeline);
	    }
	}
}