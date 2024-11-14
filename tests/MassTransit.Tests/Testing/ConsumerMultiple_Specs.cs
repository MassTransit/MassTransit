namespace MassTransit.Tests.Testing
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;


    public class When_a_consumer_with_multiple_message_consumers_is_tested
    {
        ConsumerTestHarness<TwoMessageConsumer> _consumer;
        InMemoryTestHarness _harness;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _consumer = _harness.Consumer<TwoMessageConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A(), context => context.ResponseAddress = _harness.BusAddress);
            await _harness.InputQueueSendEndpoint.Send(new B(), context => context.ResponseAddress = _harness.BusAddress);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
            _harness.Dispose();
        }

        [Test]
        public void Should_have_sent_the_aa_response_from_the_consumer()
        {
            Assert.That(_harness.Sent.Select<Aa>().Any(), Is.True);
        }

        [Test]
        public void Should_have_sent_the_bb_response_from_the_consumer()
        {
            Assert.That(_harness.Sent.Select<Bb>().Any(), Is.True);
        }

        [Test]
        public void Should_have_called_the_consumer_a_method()
        {
            Assert.That(_consumer.Consumed.Select<A>().Any(), Is.True);
        }

        [Test]
        public void Should_have_called_the_consumer_b_method()
        {
            Assert.That(_consumer.Consumed.Select<B>().Any(), Is.True);
        }


        class A
        {
        }


        class Aa
        {
        }


        class B
        {
        }


        class Bb
        {
        }


        class TwoMessageConsumer :
            IConsumer<A>,
            IConsumer<B>
        {
            public Task Consume(ConsumeContext<A> context)
            {
                return context.RespondAsync(new Aa());
            }

            public Task Consume(ConsumeContext<B> context)
            {
                return context.RespondAsync(new Bb());
            }
        }
    }
}
