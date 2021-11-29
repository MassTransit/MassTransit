namespace MassTransit.GrpcTransport.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class When_a_consumer_is_being_tested
    {
        [Test]
        public void Should_have_called_the_consumer_method()
        {
            Assert.IsTrue(_consumer.Consumed.Select<A>().Any());
        }

        [Test]
        public void Should_have_sent_the_response_from_the_consumer()
        {
            Assert.IsTrue(_harness.Published.Select<B>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.IsTrue(_harness.Consumed.Select<A>().Any());
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Sent.Select<A>().Any());
        }

        InMemoryTestHarness _harness;
        ConsumerTestHarness<TestConsumer> _consumer;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _consumer = _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }


        class TestConsumer :
            IConsumer<A>
        {
            public async Task Consume(ConsumeContext<A> context)
            {
                await context.RespondAsync(new B());
            }
        }


        class A
        {
        }


        class B
        {
        }
    }


    [TestFixture]
    public class When_a_slow_consumer_is_being_tested
    {
        [Test]
        public void Should_have_called_the_consumer_method()
        {
            Assert.That(async () => await _consumer.Consumed.Any<A>(), Is.True);
        }

        [Test]
        public void Should_have_sent_the_response_from_the_consumer()
        {
            Assert.That(async () => await _harness.Published.Any<B>(), Is.True);
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.That(async () => await _harness.Consumed.Any<A>(), Is.True);
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.That(async () => await _harness.Sent.Any<A>(), Is.True);
        }

        InMemoryTestHarness _harness;
        ConsumerTestHarness<TestConsumer> _consumer;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _consumer = _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }


        class TestConsumer :
            IConsumer<A>
        {
            public async Task Consume(ConsumeContext<A> context)
            {
                await Task.Delay(2000);
                await context.RespondAsync(new B());
            }
        }


        class A
        {
        }


        class B
        {
        }
    }


    [TestFixture]
    public class When_a_consumer_of_interfaces_is_being_tested
    {
        [Test]
        public void Should_have_called_the_consumer_method()
        {
            Assert.IsTrue(_consumer.Consumed.Select<IA>().Any());
        }

        [Test]
        public void Should_have_sent_the_response_from_the_consumer()
        {
            Assert.IsTrue(_harness.Published.Select<B>().Any());
            Assert.IsTrue(_harness.Published.Select<IB>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.IsTrue(_harness.Consumed.Select<IA>().Any());
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Sent.Select<A>().Any());
            Assert.IsTrue(_harness.Sent.Select<IA>().Any());
        }

        InMemoryTestHarness _harness;
        ConsumerTestHarness<TestInterfaceConsumer> _consumer;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _consumer = _harness.Consumer<TestInterfaceConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }


        class TestInterfaceConsumer :
            IConsumer<IA>
        {
            public async Task Consume(ConsumeContext<IA> context)
            {
                await context.RespondAsync(new B());
            }
        }


        public interface IA
        {
        }


        class A :
            IA
        {
        }


        public interface IB
        {
        }


        class B :
            IB
        {
        }
    }


    public class When_a_context_consumer_is_being_tested
    {
        ConsumerTestHarness<TestConsumer> _consumer;
        InMemoryTestHarness _harness;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _consumer = _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A(), context => context.ResponseAddress = _harness.BusAddress);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Sent.Select<A>().Any());
        }

        [Test]
        public void Should_have_sent_the_response_from_the_consumer()
        {
            Assert.IsTrue(_harness.Sent.Select<B>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.IsTrue(_harness.Consumed.Select<A>().Any());
        }

        [Test]
        public void Should_have_called_the_consumer_method()
        {
            Assert.IsTrue(_consumer.Consumed.Select<A>().Any());
        }


        class TestConsumer :
            IConsumer<A>
        {
            public Task Consume(ConsumeContext<A> context)
            {
                return context.RespondAsync(new B());
            }
        }


        class A
        {
        }


        class B
        {
        }
    }
}
