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
	using Rhino.Mocks;
	using TestConsumers;

	[TestFixture]
	public class When_filtering_messages_on_the_pipeline
	{
		[SetUp]
		public void Setup()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
		}

		private IObjectBuilder _builder;

		[Test]
		public void A_filter_should_be_nameable()
		{
			MessagePipeline pipeline = MessagePipelineConfigurator.CreateDefault(_builder);

			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();

			pipeline.Filter<PingMessage>("cock blocker", x => false);

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			consumer.ShouldNotHaveReceivedMessage(message);

			PipelineViewer.Trace(pipeline);
		}

		[Test]
		[Ignore("This is a planned feature, but is not yet functional.")]
		public void A_filter_should_be_removable()
		{
			MessagePipeline pipeline = MessagePipelineConfigurator.CreateDefault(_builder);

			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();

			var f = pipeline.Filter<PingMessage>(x => false);
			PipelineViewer.Trace(pipeline);

			PingMessage message = new PingMessage();
			pipeline.Dispatch(message);

			consumer.ShouldNotHaveReceivedMessage(message);

			f();
			PipelineViewer.Trace(pipeline);

			message = new PingMessage();
			pipeline.Dispatch(message);

			consumer.ShouldHaveReceivedMessage(message);
		}

		[Test]
		public void A_filtered_message_should_not_be_received()
		{
			MessagePipeline pipeline = MessagePipelineConfigurator.CreateDefault(_builder);

			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();

			pipeline.Filter<PingMessage>(x => false);

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			consumer.ShouldNotHaveReceivedMessage(message);

			PipelineViewer.Trace(pipeline);
		}

		[Test]
		public void A_message_should_fall_throuh_happy_filters()
		{
			MessagePipeline pipeline = MessagePipelineConfigurator.CreateDefault(_builder);

			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();

			pipeline.Filter<PingMessage>(x => true);

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			consumer.ShouldHaveReceivedMessage(message);

			PipelineViewer.Trace(pipeline);
		}

		[Test]
		public void An_unfiltered_message_should_be_received()
		{
			MessagePipeline pipeline = MessagePipelineConfigurator.CreateDefault(_builder);

			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			consumer.ShouldHaveReceivedMessage(message);
		}
	}
}