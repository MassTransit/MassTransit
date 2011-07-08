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
	public class When_subscription_a_component_to_the_pipeline
	{
		[SetUp]
		public void Setup()
		{
			_pipeline = InboundPipelineConfigurator.CreateDefault(null);
		}

		private IInboundMessagePipeline _pipeline;

		[Test]
		public void The_appropriate_handler_should_be_added()
		{
			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			_pipeline.ConnectInstance(consumer);

			PingMessage message = new PingMessage();

			_pipeline.Dispatch(message, x => true);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void The_pipeline_should_have_insertable_items()
		{
			// two consumers, one for each type of message

			IndiscriminantConsumer<PingMessage> pingConsumer = new IndiscriminantConsumer<PingMessage>();
			IndiscriminantConsumer<PongMessage> pongConsumer = new IndiscriminantConsumer<PongMessage>();

			UnsubscribeAction pingToken = _pipeline.ConnectInstance(pingConsumer);
			UnsubscribeAction pongToken = _pipeline.ConnectInstance(pongConsumer);

			PipelineViewer.Trace(_pipeline);

			PingMessage pingMessage = new PingMessage();
			PongMessage pongMessage = new PongMessage();

			_pipeline.Dispatch(pingMessage, accept => true);
			_pipeline.Dispatch(pongMessage, accept => true);

			Assert.AreEqual(pingMessage, pingConsumer.Consumed);
			Assert.AreEqual(pongMessage, pongConsumer.Consumed);

			pingToken();
			pongToken();

			PipelineViewer.Trace(_pipeline);
		}

		[Test]
		public void The_subscriptions_should_be_a_separate_concern_from_the_pipeline()
		{
			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			_pipeline.ConnectInstance(consumer);

			PingMessage message = new PingMessage();

			_pipeline.Dispatch(message, x => true);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void When_nobody_wants_the_message_it_should_not_be_accepted()
		{
			PingMessage message = new PingMessage();

			bool accepted = false;

			_pipeline.Dispatch(message, x => accepted = true);

			Assert.IsFalse(accepted);
		}

		[Test]
		public void When_somebody_gets_the_message_it_should_be_accepted()
		{
			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			_pipeline.ConnectInstance(consumer);

			PingMessage message = new PingMessage();

			bool accepted = false;
			_pipeline.Dispatch(message, x => accepted = true);

			Assert.IsTrue(accepted);
		}
	}
}