namespace MassTransit.Tests.Testing
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;


    public class When_a_handler_responds_to_a_message
    {
        HandlerTestHarness<A> _handler;
        InMemoryTestHarness _harness;

        [OneTimeSetUp]
        public async Task A_handler_responds_to_a_message()
        {
            _harness = new InMemoryTestHarness();
            _handler = _harness.Handler<A>(async context => await context.RespondAsync(new B()));

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A(), x => x.ResponseAddress = _harness.BusAddress);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
            _harness.Dispose();
        }

        [Test]
        public async Task Should_have_sent_a_message_of_type_b()
        {
            Assert.That(await _harness.Sent.Any<B>());
        }

        [Test]
        public async Task Should_have_sent_message_to_bus_address()
        {
            ISentMessage<B> message = await _harness.Sent.SelectAsync<B>().First();
            Assert.That(message, Is.Not.Null);

            Assert.That(message.Context.DestinationAddress, Is.EqualTo(_harness.BusAddress));
        }

        [Test]
        public void Should_support_a_simple_handler()
        {
            Assert.That(_handler.Consumed.Select().Any(), Is.True);
        }


        class A
        {
        }


        class B
        {
        }
    }
}
