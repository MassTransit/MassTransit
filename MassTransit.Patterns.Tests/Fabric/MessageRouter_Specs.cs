namespace MassTransit.Patterns.Tests.Fabric
{
	using MassTransit.Patterns.Fabric;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using ServiceBus;

	[TestFixture]
	public class When_the_message_router_consumes_a_message
	{
		[Test]
		public void The_message_should_be_routed_to_all_consumers()
		{
			RoutedMessageConsumer consumer1 = new RoutedMessageConsumer();
			RoutedMessageConsumer consumer2 = new RoutedMessageConsumer();
			using (MessageRouter<RoutedMessage> router = new MessageRouter<RoutedMessage>(consumer1, consumer2))
			{
				RoutedMessage message = new RoutedMessage(427);
				router.Consume(message);

				Assert.That(consumer1.Value, Is.EqualTo(427));
				Assert.That(consumer2.Value, Is.EqualTo(427));
			}
		}
	}

	public class RoutedMessage : IMessage
	{
		private int _value;

		public RoutedMessage(int value)
		{
			_value = value;
		}

		public int Value
		{
			get { return _value; }
			set { _value = value; }
		}
	}

	public class RoutedMessageConsumer : Patterns.Fabric.IConsume<RoutedMessage>
	{
		private int _value;

		public int Value
		{
			get { return _value; }
		}

		public void Consume(RoutedMessage message)
		{
			_value = message.Value;
		}

		public void Dispose()
		{
		}
	}
}