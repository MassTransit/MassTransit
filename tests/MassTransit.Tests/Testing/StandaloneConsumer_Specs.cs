namespace MassTransit.Tests.Testing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class StandaloneConsumer_Specs
    {
        [Test]
        public async Task Should_be_able_to_create_standalone_consumer_test_in_memory()
        {
            var harness = new InMemoryTestHarness();
            ConsumerTestHarness<MyConsumer> consumer = harness.Consumer<MyConsumer>();

            await harness.Start();
            try
            {
                await harness.InputQueueSendEndpoint.Send(new PingMessage());

                Assert.Multiple(() =>
                {
                    Assert.That(harness.Consumed.Select<PingMessage>().Any(), Is.True);

                    Assert.That(consumer.Consumed.Select<PingMessage>().Any(), Is.True);
                });
            }
            finally
            {
                await harness.Stop();
            }
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
