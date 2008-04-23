namespace MassTransit.Patterns.Tests.Fabric
{
	using MassTransit.Patterns.Fabric;
	using NUnit.Framework;

	[TestFixture]
	public class When_connected_to_a_consumer
	{
		public class MyConsumer : Consumes<MyMessage>
		{
			#region Consumes<MyMessage> Members

			public void Consume(MyMessage message)
			{
			}

			#endregion
		}

		public class MyMessage
		{
		}

		[Test]
		public void The_message_should_be_consumed_by_the_object()
		{
			MyConsumer consumer = new MyConsumer();

			MyMessage message = new MyMessage();

			consumer.Consume(message);
		}
	}
}