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
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Pipeline;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestConsumers;

	[TestFixture]
	public class When_subscribing_a_consumer_to_the_pipeline
	{
		[SetUp]
		public void Setup()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();

		}

		private IObjectBuilder _builder;

		[Test]
		public void A_bunch_of_mixed_subscriber_types_should_work()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline(_builder);

			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();
			ParticularConsumer consumerYes = new ParticularConsumer(true);
			ParticularConsumer consumerNo = new ParticularConsumer(false);

			var unsubscribeToken = pipeline.Subscribe(consumer);
			unsubscribeToken += pipeline.Subscribe(consumerYes);
			unsubscribeToken += pipeline.Subscribe(consumerNo);

			PipelineViewer.Trace(pipeline);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(message, consumer.Consumed);
			Assert.AreEqual(message, consumerYes.Consumed);
			Assert.AreEqual(null, consumerNo.Consumed);

			unsubscribeToken();

			PingMessage nextMessage = new PingMessage();
			pipeline.Dispatch(nextMessage);

			Assert.AreEqual(message, consumer.Consumed);
			Assert.AreEqual(message, consumerYes.Consumed);
		}

		[Test]
		public void A_component_should_be_subscribed_to_the_pipeline()
		{
			TestMessageConsumer<PingMessage> consumer = MockRepository.GenerateMock<TestMessageConsumer<PingMessage>>();

			_builder.Expect(x => x.GetInstance<TestMessageConsumer<PingMessage>>()).Return(consumer);

			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline(_builder);

			pipeline.Subscribe<TestMessageConsumer<PingMessage>>();

			PingMessage message = new PingMessage();

			consumer.Expect(x => x.Consume(message));

			pipeline.Dispatch(message);

            consumer.VerifyAllExpectations();
		}

		[Test]
		public void The_subscription_should_be_added()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline(_builder);

			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void The_subscription_should_be_added_for_selective_consumers()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline(_builder);

			ParticularConsumer consumer = new ParticularConsumer(false);

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(null, consumer.Consumed);
		}

		[Test]
		public void The_subscription_should_be_added_for_selective_consumers_that_are_interested()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline(_builder);

			ParticularConsumer consumer = new ParticularConsumer(true);

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void The_wrong_type_of_message_should_not_blow_up_the_test()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline(_builder);

			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			pipeline.Subscribe(consumer);

			PongMessage message = new PongMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(null, consumer.Consumed);
		}
	}
}