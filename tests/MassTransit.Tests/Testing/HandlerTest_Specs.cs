namespace MassTransit.Tests.Testing
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class Using_the_handler_test_factory
    {
        [Test]
        public void Should_have_received_a_message_of_type_a()
        {
            Assert.That(_harness.Consumed.Select<A>().Any(), Is.True);
        }

        [Test]
        public void Should_have_sent_a_message_of_type_a()
        {
            Assert.That(_harness.Sent.Select<A>().Any(), Is.True);
        }

        [Test]
        public void Should_have_sent_a_message_of_type_b()
        {
            Assert.That(_harness.Sent.Select<B>().Any(), Is.True);
        }

        [Test]
        public void Should_support_a_simple_handler()
        {
            Assert.That(_handler.Consumed.Select().Any(), Is.True);
        }

        InMemoryTestHarness _harness;
        HandlerTestHarness<A> _handler;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _harness = new InMemoryTestHarness();
            _handler = _harness.Handler<A>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
            await _harness.InputQueueSendEndpoint.Send(new B());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
            _harness.Dispose();
        }


        class A
        {
        }


        class B
        {
        }
    }


    [TestFixture]
    public class Publishing_to_a_handler_test
    {
        [Test]
        public void Should_have_published_a_message_of_type_b()
        {
            Assert.That(_harness.Published.Select<B>().Any(), Is.True);
        }

        [Test]
        public void Should_have_published_a_message_of_type_ib()
        {
            Assert.That(_harness.Published.Select<IB>().Any(), Is.True);
        }

        [Test]
        public void Should_have_received_a_message_of_type_a()
        {
            Assert.That(_harness.Consumed.Select<A>().Any(), Is.True);
        }

        [Test]
        public void Should_have_sent_a_message_of_type_a()
        {
            Assert.That(_harness.Published.Select<A>().Any(), Is.True);
        }

        [Test]
        public void Should_support_a_simple_handler()
        {
            Assert.That(_handler.Consumed.Select().Any(), Is.True);
        }

        InMemoryTestHarness _harness;
        HandlerTestHarness<A> _handler;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _harness = new InMemoryTestHarness();
            _handler = _harness.Handler<A>();

            await _harness.Start();

            await _harness.Bus.Publish(new A());
            await _harness.Bus.Publish(new B());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
            _harness.Dispose();
        }


        class A
        {
        }


        class B :
            IB
        {
        }


        interface IB
        {
        }
    }
}
