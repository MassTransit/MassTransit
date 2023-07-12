namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    [Category("Flaky")]
    public class StartStop_Specs :
        BusTestFixture
    {
        [Test]
        public async Task Should_start_stop_and_start()
        {
            var bus = MassTransit.Bus.Factory.CreateUsingAmazonSqs(x =>
            {
                x.AutoStart = true;

                x.Host(new Uri("amazonsqs://localhost:4566"), h =>
                {
                    h.AccessKey("admin");
                    h.SecretKey("admin");
                    h.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = "http://localhost:4566" });
                    h.Config(new AmazonSQSConfig { ServiceURL = "http://localhost:4566" });
                });

                ConfigureBusDiagnostics(x);

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Handler<PingMessage>(async context =>
                    {
                        await context.RespondAsync(new PongMessage());
                    });
                });
            });

            await Task.Delay(3000);

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
                await Task.Delay(1000);

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
