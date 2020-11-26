namespace MassTransit.RabbitMqTransport.Tests
{
    namespace AmazonMqTests
    {
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Messages;


        [TestFixture]
        public class Connecting_to_RabbitMQ_via_Amazon :
            InMemoryTestFixture
        {
            [Test]
            [Explicit]
            public async Task Should_connect()
            {
                var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri(Configuration.AmazonRabbitMqHost), h =>
                    {
                        h.Username(Configuration.AmazonRabbitMqUser);
                        h.Password(Configuration.AmazonRabbitMqPass);
                    });

                    ConfigureBusDiagnostics(cfg);

                    cfg.ReceiveEndpoint("input-queue", e =>
                    {
                        e.Handler<PingMessage>(async context =>
                        {
                        });
                    });
                });

                await bus.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);
                try
                {
                }
                finally
                {
                    await bus.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);
                }
            }
        }
    }
}
