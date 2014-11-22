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
namespace MassTransit.Tests.Pipeline
{
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using Messages;
	using NUnit.Framework;
	using TestFramework.Messages;


    [TestFixture]
	public class When_building_a_pipeline
	{
		[SetUp]
		public void Setup()
		{
			_pipeline = InboundPipelineConfigurator.CreateDefault(null);
		}

		IInboundMessagePipeline _pipeline;

		[Test]
		public void The_pipeline_should_be_happy()
		{
			var consumer = new IndiscriminantConsumer<PingMessage>();

			_pipeline.ConnectInstance(consumer);

			_pipeline.Dispatch(new PingMessage());

			Assert.IsNotNull(consumer.Consumed);
		}
	}
}