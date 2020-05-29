namespace MassTransit.Tests.Testing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_a_consumer_test_harness :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_able_to_create_standalone_consumer_test_in_memory()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            Assert.That(_consumer.Consumed.Select<PingMessage>().Any(), Is.True);
        }

        readonly ConsumerTestHarness<MyConsumer> _consumer;

        public Using_a_consumer_test_harness()
        {
            _consumer = BusTestHarness.Consumer<MyConsumer>();
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Console.Out.WriteLineAsync("Pinged");
            }
        }
    }
}
