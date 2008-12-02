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
	}
}