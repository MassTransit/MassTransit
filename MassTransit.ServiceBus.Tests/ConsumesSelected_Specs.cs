namespace MassTransit.ServiceBus.Tests
{
	using System;
	using MassTransit.ServiceBus.Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_a_message_is_received_for_a_selective_consumer :
        Specification
	{
		private MessageTypeDispatcher _dispatcher;
		private TestMessage _message;
		private readonly int _value = 27;
		private IObjectBuilder _builder;
		private SubscriptionCoordinator _coordinator;

	    protected override void Before_each()
		{
	        _builder = StrictMock<IObjectBuilder>();

			_dispatcher = new MessageTypeDispatcher();
	    	_coordinator = new SubscriptionCoordinator(_dispatcher, null, null, _builder);
			_message = new TestMessage(_value);
		}


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

		internal class GeneralConsumer : Consumes<TestMessage>.All
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

			_coordinator.Resolve(consumerA).Subscribe(consumerA);

			GeneralConsumer consumerB = new GeneralConsumer();
			_coordinator.Resolve(consumerB).Subscribe(consumerB);

			_dispatcher.Consume(_message);

			Assert.That(consumerA.Value, Is.EqualTo(default(int)));
			Assert.That(consumerB.Value, Is.EqualTo(_value));
		}

		[Test]
		public void It_should_only_be_dispatched_to_interested_consumers_again()
		{
			TestConsumer consumerA = new TestConsumer(delegate(TestMessage message) { return message.Value >= 32; });
			_coordinator.Resolve(consumerA).Subscribe(consumerA);

			TestConsumer consumerB = new TestConsumer(delegate(TestMessage message) { return message.Value < 32; });
			_coordinator.Resolve(consumerB).Subscribe(consumerB);

			_dispatcher.Consume(_message);

			Assert.That(consumerA.Value, Is.EqualTo(default(int)));
			Assert.That(consumerB.Value, Is.EqualTo(_value));
		}
	}
}