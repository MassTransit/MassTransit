namespace MassTransit.ServiceBus.Tests
{
	using Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_a_consumer_is_registered_with_the_dispatcher :
        Specification
	{
		#region Setup/Teardown

	    protected override void Before_each()
		{
			_dispatcher = new MessageDispatcher<TestMessage>();
			_message = new TestMessage(_value);
		}

		#endregion

		private MessageDispatcher<TestMessage> _dispatcher;
		private TestMessage _message;
		private readonly int _value = 27;

		internal class TestConsumer : Consumes<TestMessage>.All
		{
			private int _value;

			public int Value
			{
				get { return _value; }
			}

			#region All Members

			public void Consume(TestMessage message)
			{
				_value = message.Value;
			}

			#endregion
		}

		internal class TestMessage
		{
			private readonly int _value;

			public TestMessage(int value)
			{
				_value = value;
			}

			public int Value
			{
				get { return _value; }
			}
		}

		[Test]
		public void If_a_consumer_is_no_longer_subscribed_it_should_not_be_called()
		{
			TestConsumer consumerA = new TestConsumer();
			_dispatcher.Attach(consumerA);

			_dispatcher.Detach(consumerA);

			_dispatcher.Consume(_message);

			Assert.That(consumerA.Value, Is.EqualTo(default(int)));
		}

		[Test]
		public void If_a_consumer_is_no_longer_subscribed_only_the_remaining_consumers_should_be_called()
		{
			TestConsumer consumerA = new TestConsumer();
			_dispatcher.Attach(consumerA);

			TestConsumer consumerB = new TestConsumer();
			_dispatcher.Attach(consumerB);

			_dispatcher.Detach(consumerA);

			_dispatcher.Consume(_message);

			Assert.That(consumerA.Value, Is.EqualTo(default(int)));
			Assert.That(consumerB.Value, Is.EqualTo(_value));
		}

		[Test]
		public void If_multiple_consumers_are_registered_they_should_all_be_called()
		{
			TestConsumer consumerA = new TestConsumer();
			_dispatcher.Attach(consumerA);

			TestConsumer consumerB = new TestConsumer();
			_dispatcher.Attach(consumerB);

			_dispatcher.Consume(_message);

			Assert.That(consumerA.Value, Is.EqualTo(_value));
			Assert.That(consumerB.Value, Is.EqualTo(_value));
		}

		[Test]
		public void It_should_be_called_when_a_message_is_dispatched()
		{
			TestConsumer consumerA = new TestConsumer();
			_dispatcher.Attach(consumerA);

			_dispatcher.Consume(_message);

			Assert.That(consumerA.Value, Is.EqualTo(_message.Value));
		}
	}
}