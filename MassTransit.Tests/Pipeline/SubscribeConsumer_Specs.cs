namespace MassTransit.Tests.Pipeline
{
	using MassTransit.Pipeline;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class When_subscribing_a_consumer_to_the_pipeline
	{
		[Test]
		public void The_subscription_should_be_added()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline();

			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			var token = pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void The_wrong_type_of_message_should_not_blow_up_the_test()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline();

			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			var token = pipeline.Subscribe(consumer);

			PongMessage message = new PongMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(null, consumer.Consumed);
		}

		[Test]
		public void The_subscription_should_be_added_for_selective_consumers()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline();

			ParticularConsumer consumer = new ParticularConsumer(false);

			var token = pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(null, consumer.Consumed);
		}

		[Test]
		public void The_subscription_should_be_added_for_selective_consumers_that_are_interested()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline();

			ParticularConsumer consumer = new ParticularConsumer(true);

			var token = pipeline.Subscribe(consumer);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void A_bunch_of_mixed_subscriber_types_should_work()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline();

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
	}
}