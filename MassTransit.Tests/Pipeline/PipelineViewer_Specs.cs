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
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_working_with_an_existing_pipeline
	{
		[Test]
		public void I_want_to_display_the_entire_flow_through_the_pipeline()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline(MockRepository.GenerateMock<IObjectBuilder>());

			PipelineViewer.Trace(pipeline);
		}
	}
}