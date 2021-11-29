namespace MassTransit.RabbitMqTransport.Tests.Testing
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Using_the_handler_test_factory
    {
        [Test]
        public void Should_have_published_a_message_of_type_b()
        {
            _harness.Published.Select<B>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_published_a_message_of_type_d()
        {
            _harness.Published.Select<D>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_received_a_message_of_type_a()
        {
            _harness.Consumed.Select<A>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_sent_a_message_of_type_a()
        {
            _harness.Sent.Select<A>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_sent_a_message_of_type_c()
        {
            _harness.Sent.Select<C>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_support_a_simple_handler()
        {
            _handler.Consumed.Select().Any().ShouldBe(true);
        }

        RabbitMqTestHarness _harness;
        HandlerTestHarness<A> _handler;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _harness = new RabbitMqTestHarness();
            _handler = _harness.Handler<A>(async context =>
            {
                var endpoint = await context.GetSendEndpoint(context.SourceAddress);

                await endpoint.Send(new C());

                await context.Publish(new D());
            });

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
            await _harness.Bus.Publish(new B());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }


        class A
        {
        }


        class B
        {
        }


        class C
        {
        }


        class D
        {
        }
    }
}
