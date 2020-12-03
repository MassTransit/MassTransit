namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class StartStop_Specs :
        BusTestFixture
    {
        [Test]
        public async Task Should_start_stop_and_start()
        {
            var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host("localhost", "test");

                ConfigureBusDiagnostics(x);

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.PurgeOnStartup = true;

                    e.Handler<PingMessage>(async context =>
                    {
                        await context.RespondAsync(new PongMessage());
                    });
                });
            });

            await bus.StartAsync(TestCancellationToken);
            try
            {
                await bus.CreateRequestClient<PingMessage>().GetResponse<PongMessage>(new PingMessage());
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }

            await bus.StartAsync(TestCancellationToken);
            try
            {
                await bus.CreateRequestClient<PingMessage>().GetResponse<PongMessage>(new PingMessage());
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }
        }

        public StartStop_Specs()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
