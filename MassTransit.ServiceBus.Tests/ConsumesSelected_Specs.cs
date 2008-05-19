namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_a_message_is_received_for_a_selective_consumer
	{
		[SetUp]
		public void Before_each()
		{
			_dispatcher = new MessageDispatcher();
			_message = new TestMessage(_value);
		}

		private MessageDispatcher _dispatcher;
		private TestMessage _message;
		private readonly int _value = 27;

		internal class InvalidConsumer
		{
		}

		internal class TestConsumer : Consumes<TestMessage>.Selected
		{
			private int _value;
			private readonly Predicate<TestMessage> _accept;

			public TestConsumer(Predicate<TestMessage> accept)
			{
				_accept = accept;
			}

			public int Value
			{
				get { return _value; }
			}

			public void Consume(TestMessage message)
			{
				_value = message.Value;
			}

			public bool Accept(TestMessage message)
			{
				return _accept(message);
			}
		}

		internal class TestMessage : CorrelatedBy<Guid>
		{
			private readonly Guid _correlationId;
			private readonly int _value;

			public TestMessage(int value)
			{
				_value = value;
				_correlationId = Guid.NewGuid();
			}

			public int Value
			{
				get { return _value; }
			}

			public Guid CorrelationId
			{
				get { return _correlationId; }
			}
		}

		internal class GeneralConsumer : Consumes<TestMessage>.Any
		{
			private int _value;

			public int Value
			{
				get { return _value; }
			}

			public void Consume(TestMessage message)
			{
				_value = message.Value;
			}
		}

		[Test]
		public void It_should_only_be_dispatched_to_interested_consumers()
		{
			TestConsumer consumerA = new TestConsumer(delegate(TestMessage message) { return message.Value >= 32; });
			_dispatcher.Subscribe(consumerA);

			GeneralConsumer consumerB = new GeneralConsumer();
			_dispatcher.Subscribe(consumerB);

			_dispatcher.Consume(_message);

			Assert.That(consumerA.Value, Is.EqualTo(default(int)));
			Assert.That(consumerB.Value, Is.EqualTo(_value));
		}

		[Test]
		public void It_should_only_be_dispatched_to_interested_consumers_again()
		{
			TestConsumer consumerA = new TestConsumer(delegate(TestMessage message) { return message.Value >= 32; });
			_dispatcher.Subscribe(consumerA);

			TestConsumer consumerB = new TestConsumer(delegate(TestMessage message) { return message.Value < 32; });
			_dispatcher.Subscribe(consumerB);

			_dispatcher.Consume(_message);

			Assert.That(consumerA.Value, Is.EqualTo(default(int)));
			Assert.That(consumerB.Value, Is.EqualTo(_value));
		}
	}
}