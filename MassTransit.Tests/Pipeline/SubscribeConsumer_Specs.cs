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
	using System;
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Inspectors;
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
			InboundPipeline pipeline = new InboundPipeline(_builder);

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

			_builder.Expect(x => x.GetInstance<TestMessageConsumer<PingMessage>>()).Return(consumer).Repeat.Once();

			InboundPipeline pipeline = new InboundPipeline(_builder);

			pipeline.Subscribe<TestMessageConsumer<PingMessage>>();

			PipelineViewer.Trace(pipeline);

			PingMessage message = new PingMessage();
			consumer.Expect(x => x.Consume(message));

			pipeline.Dispatch(message);

			consumer.VerifyAllExpectations();
			_builder.VerifyAllExpectations();
		}

		[Test]
		public void A_selective_component_should_properly_handle_the_love()
		{
			ParticularConsumer consumer = MockRepository.GenerateMock<ParticularConsumer>();

			_builder.Expect(x => x.GetInstance<ParticularConsumer>()).Return(consumer).Repeat.Once();

			InboundPipeline pipeline = new InboundPipeline(_builder);

			pipeline.Subscribe<ParticularConsumer>();

			PipelineViewer.Trace(pipeline);

			PingMessage message = new PingMessage();
			consumer.Expect(x => x.Accept(message)).Return(true);
			consumer.Expect(x => x.Consume(message));

			pipeline.Dispatch(message);

			consumer.VerifyAllExpectations();
			_builder.VerifyAllExpectations();
		}

		[Test]
		public void A_component_should_be_subscribed_to_multiple_messages_on_the_pipeline()
		{
			PingPongConsumer consumer = MockRepository.GenerateMock<PingPongConsumer>();

			_builder.Expect(x => x.GetInstance<PingPongConsumer>()).Return(consumer).Repeat.Twice();

			InboundPipeline pipeline = new InboundPipeline(_builder);

			pipeline.Subscribe<PingPongConsumer>();

			PipelineViewer.Trace(pipeline);

			PingMessage ping = new PingMessage();
			consumer.Expect(x => x.Consume(ping));
			pipeline.Dispatch(ping);

			PongMessage pong = new PongMessage(ping.CorrelationId);
			consumer.Expect(x => x.Consume(pong));
			pipeline.Dispatch(pong);

			_builder.VerifyAllExpectations();
			consumer.VerifyAllExpectations();
		}

		[Test]
		public void The_subscription_should_be_added()
		{
			InboundPipeline pipeline = new InboundPipeline(_builder);

			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void Correlated_subscriptions_should_make_happy_sounds()
		{
			InboundPipeline pipeline = new InboundPipeline(_builder);

			PingMessage message = new PingMessage();

			TestCorrelatedConsumer<PingMessage, Guid> consumer = new TestCorrelatedConsumer<PingMessage, Guid>(message.CorrelationId);
			TestCorrelatedConsumer<PingMessage, Guid> negativeConsumer = new TestCorrelatedConsumer<PingMessage, Guid>(Guid.Empty);

			Func<bool> token = pipeline.Subscribe(consumer);
			token += pipeline.Subscribe(negativeConsumer);

			PipelineViewer.Trace(pipeline);

			pipeline.Dispatch(message);

			consumer.ShouldHaveReceivedMessage(message, 0.Seconds());
			negativeConsumer.ShouldNotHaveReceivedMessage(message, 0.Seconds());

			token();

			PipelineViewer.Trace(pipeline);
		}


		[Test]
		public void The_subscription_should_be_added_for_selective_consumers()
		{
			InboundPipeline pipeline = new InboundPipeline(_builder);

			ParticularConsumer consumer = new ParticularConsumer(false);

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(null, consumer.Consumed);
		}

		[Test]
		public void The_subscription_should_be_added_for_selective_consumers_that_are_interested()
		{
			InboundPipeline pipeline = new InboundPipeline(_builder);

			ParticularConsumer consumer = new ParticularConsumer(true);

			pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void The_wrong_type_of_message_should_not_blow_up_the_test()
		{
			InboundPipeline pipeline = new InboundPipeline(_builder);

			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			pipeline.Subscribe(consumer);

			PongMessage message = new PongMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(null, consumer.Consumed);
		}
	}
}