namespace MassTransit.Tests.Testing
{
	using Magnum.TestFramework;
	using MassTransit.Testing;

	[Scenario]
	public class When_a_consumer_with_multiple_message_consumers_is_tested
	{
		ConsumerTest<Testsumer> _test;

		[When]
		public void A_consumer_is_being_tested()
		{
			_test = TestFactory.ForConsumer<Testsumer>()
				.New(x =>
				{
					x.ConstructUsing(() => new Testsumer());

					x.Send(new A(), c => c.SendResponseTo(_test.Scenario.Bus));
					x.Send(new B(), c => c.SendResponseTo(_test.Scenario.Bus));
				});

			_test.Execute();
		}

		[Finally]
		public void Teardown()
		{
			_test.Dispose();
			_test = null;
		}

		[Then]
		public void Should_have_sent_the_aa_response_from_the_consumer()
		{
			_test.Sent.Any<Aa>().ShouldBeTrue();
		}

		[Then]
		public void Should_have_sent_the_bb_response_from_the_consumer()
		{
			_test.Sent.Any<Bb>().ShouldBeTrue();
		}

		[Then]
		public void Should_have_called_the_consumer_a_method()
		{
			_test.Consumer.Received.Any<A>().ShouldBeTrue();
		}

		[Then]
		public void Should_have_called_the_consumer_b_method()
		{
			_test.Consumer.Received.Any<B>().ShouldBeTrue();
		}

		class Testsumer :
			Consumes<A>.All,
			Consumes<B>.All
		{
			public void Consume(A message)
			{
				this.MessageContext<A>().Respond(new Aa());
			}

			public void Consume(B message)
			{
				this.MessageContext<B>().Respond(new Bb());
			}
		}

		class A
		{
		}
		class Aa
		{}

		class B
		{
		}
		class Bb{}
	}
}
