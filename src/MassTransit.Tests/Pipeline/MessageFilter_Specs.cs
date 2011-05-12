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
	using TestConsumers;

//	[TestFixture]
//	public class When_filtering_messages_on_the_pipeline
//	{
//		[SetUp]
//		public void Setup()
//		{
//			_pipeline = InboundPipelineConfigurator.CreateDefault(null);
//		}
//
//		private MessagePipeline _pipeline;
//
//		[Test]
//		public void A_filter_should_be_nameable()
//		{
//			var consumer = new TestMessageConsumer<PingMessage>();
//
//			_pipeline.Filter<PingMessage>("Message Blocker", x => false);
//
//			_pipeline.ConnectInstance(consumer);
//
//			var message = new PingMessage();
//
//			_pipeline.Dispatch(message);
//
//			consumer.ShouldNotHaveReceivedMessage(message);
//
//			PipelineViewer.Trace(_pipeline);
//		}
//
//		[Test]
//		[Ignore("This is a planned feature, but is not yet functional.")]
//		public void A_filter_should_be_removable()
//		{
//			var consumer = new TestMessageConsumer<PingMessage>();
//
//			var f = _pipeline.Filter<PingMessage>(x => false);
//			PipelineViewer.Trace(_pipeline);
//
//			var message = new PingMessage();
//			_pipeline.Dispatch(message);
//
//			consumer.ShouldNotHaveReceivedMessage(message);
//
//			f();
//			PipelineViewer.Trace(_pipeline);
//
//			message = new PingMessage();
//			_pipeline.Dispatch(message);
//
//			consumer.ShouldHaveReceivedMessage(message);
//		}
//
//		[Test]
//		public void A_filtered_message_should_not_be_received()
//		{
//			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
//
//			_pipeline.Filter<PingMessage>(x => false);
//
//			_pipeline.ConnectInstance(consumer);
//
//			PingMessage message = new PingMessage();
//
//			_pipeline.Dispatch(message);
//
//			consumer.ShouldNotHaveReceivedMessage(message);
//
//			PipelineViewer.Trace(_pipeline);
//		}
//
//		[Test]
//		public void A_message_should_fall_throuh_happy_filters()
//		{
//			var consumer = new TestMessageConsumer<PingMessage>();
//
//			_pipeline.Filter<PingMessage>(x => true);
//
//			_pipeline.ConnectInstance(consumer);
//
//			var message = new PingMessage();
//
//			_pipeline.Dispatch(message);
//
//			consumer.ShouldHaveReceivedMessage(message);
//
//			PipelineViewer.Trace(_pipeline);
//		}
//
//		[Test]
//		public void An_unfiltered_message_should_be_received()
//		{
//			var consumer = new TestMessageConsumer<PingMessage>();
//
//			_pipeline.ConnectInstance(consumer);
//
//			var message = new PingMessage();
//
//			_pipeline.Dispatch(message);
//
//			consumer.ShouldHaveReceivedMessage(message);
//		}
//
//	    [Test]
//	    public void I_should_be_able_to_filter_on_object()
//	    {
//            var consumer = new TestMessageConsumer<object>();
//
//            _pipeline.Filter<object>("Message Blocker", x => false);
//
//            _pipeline.ConnectInstance(consumer);
// 
//            var message = new PingMessage();
//
//            _pipeline.Dispatch(message);
//
//            consumer.ShouldNotHaveReceivedMessage(message);
//
//            PipelineViewer.Trace(_pipeline);
//	    }
//
//	    [Test]
//	    public void An_object_filter_should_pass_the_message_if_allowed()
//	    {
//            var consumer = new TestMessageConsumer<object>();
//
//            _pipeline.Filter<object>("Message Passer", x => true);
//
//            _pipeline.ConnectInstance(consumer);
// 
//            var message = new PingMessage();
//
//            _pipeline.Dispatch(message);
//
//            consumer.ShouldHaveReceivedMessage(message);
//
//            PipelineViewer.Trace(_pipeline);
//	    }
//	}
}