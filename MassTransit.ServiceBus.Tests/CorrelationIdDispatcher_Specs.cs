namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_a_correlated_message_is_dispatched
	{
		#region Setup/Teardown

		[SetUp]
		public void Before_each()
		{
			_dispatcher = new CorrelationIdDispatcher<TestMessage, Guid>();
			_message = new TestMessage(_value);
		}

		#endregion

		private CorrelationIdDispatcher<TestMessage, Guid> _dispatcher;
		private TestMessage _message;
		private readonly int _value = 27;

		internal class InvalidConsumer { }

		internal class TestConsumer : Consumes<TestMessage>.For<Guid>
		{
			private readonly Guid _correlationId;
			private int _value;

			public TestConsumer(Guid correlationId)
			{
				_correlationId = correlationId;
			}

			public int Value
			{
				get { return _value; }
			}

			#region For<Guid> Members

			public Guid CorrelationId
			{
				get { return _correlationId; }
			}

			public void Consume(TestMessage message)
			{
				_value = message.Value;
			}

			#endregion
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

			#region CorrelatedBy<Guid> Members

			public Guid CorrelationId
			{
				get { return _correlationId; }
			}

			#endregion
		}

		[Test]
		public void It_should_be_dispatched_to_the_consumer()
		{
			TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
			_dispatcher.Subscribe(consumerA);

			_dispatcher.Dispatch(_message);

			Assert.That(consumerA.Value, Is.EqualTo(_message.Value));
		}

		[Test]
		public void It_should_not_be_dispatched_if_the_correlation_does_not_match()
		{
			TestConsumer consumerA = new TestConsumer(Guid.NewGuid());
			_dispatcher.Subscribe(consumerA);

			_dispatcher.Dispatch(_message);

			Assert.That(consumerA.Value, Is.EqualTo(default(int)));
		}

		[Test]
		public void The_object_should_be_dispatched_to_the_consumer()
		{
			TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
			_dispatcher.Subscribe(consumerA);

			object obj = _message;

			_dispatcher.Dispatch(obj);

			Assert.That(consumerA.Value, Is.EqualTo(_message.Value));
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void An_invalid_consumer_should_throw_an_exception()
		{
			InvalidConsumer bogusConsumer = new InvalidConsumer();

			_dispatcher.Subscribe(bogusConsumer);
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void An_invalid_consumer_should_throw_an_exception_when_unsubscribing()
		{
			InvalidConsumer bogusConsumer = new InvalidConsumer();

			_dispatcher.Unsubscribe(bogusConsumer);
		}

		[Test]
		public void It_should_be_dispatched_to_all_consumers()
		{
			TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
			_dispatcher.Subscribe(consumerA);

			TestConsumer consumerB = new TestConsumer(_message.CorrelationId);
			_dispatcher.Subscribe(consumerB);

			_dispatcher.Dispatch(_message);

			Assert.That(consumerA.Value, Is.EqualTo(_value));
			Assert.That(consumerB.Value, Is.EqualTo(_value));
		}

		[Test]
		public void It_should_not_be_sent_to_uninterested_consumers()
		{
			TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
			_dispatcher.Subscribe(consumerA);

			TestConsumer consumerB = new TestConsumer(_message.CorrelationId);
			_dispatcher.Subscribe(consumerB);

			_dispatcher.Unsubscribe(consumerA);

			_dispatcher.Dispatch(_message);

			Assert.That(consumerA.Value, Is.EqualTo(default(int)));
			Assert.That(consumerB.Value, Is.EqualTo(_value));
		}

		[Test]
		public void It_should_not_be_sent_to_consumers_that_are_no_longer_interested()
		{
			TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
			_dispatcher.Subscribe(consumerA);

			TestConsumer consumerB = new TestConsumer(Guid.NewGuid());
			_dispatcher.Subscribe(consumerB);

			_dispatcher.Dispatch(_message);

			Assert.That(consumerA.Value, Is.EqualTo(_value));
			Assert.That(consumerB.Value, Is.EqualTo(default(int)));
		}

		[Test]
		public void It_should_only_be_sent_to_consumers_who_are_interested_in_the_specific_correlation_id()
		{
			TestMessage anotherMessage = new TestMessage(42);

			TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
			_dispatcher.Subscribe(consumerA);

			TestConsumer consumerB = new TestConsumer(anotherMessage.CorrelationId);
			_dispatcher.Subscribe(consumerB);

			_dispatcher.Dispatch(_message);
			_dispatcher.Dispatch(anotherMessage);

			Assert.That(consumerA.Value, Is.EqualTo(_value));
			Assert.That(consumerB.Value, Is.EqualTo(42));
		}

	}
}