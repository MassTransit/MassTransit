namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class Connector_Specs :
        BusTestFixture
    {
        [Test]
        public async Task Should_connect_subscription_endpoint()
        {
            var provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddMassTransit(x =>
                {
                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.LocalstackHost();
                    });
                }).BuildServiceProvider(true);

            var depot = provider.GetRequiredService<IBusDepot>();

            await depot.Start(TestCancellationToken);
            try
            {
                var bus = provider.GetRequiredService<IBus>();

                var connector = provider.GetRequiredService<IReceiveEndpointConnector>();
                var endpointNameFormatter = provider.GetService<IEndpointNameFormatter>() ?? DefaultEndpointNameFormatter.Instance;

                Task<ConsumeContext<PingMessage>> handled = null;
                var handle = connector.ConnectReceiveEndpoint(new TemporaryEndpointDefinition(), endpointNameFormatter, (context, cfg) =>
                {
                    handled = Handled<PingMessage>(cfg);
                });

                await handle.Ready;

                await bus.Publish(new PingMessage());

                await handled;
            }
            finally
            {
                await depot.Stop(TimeSpan.FromSeconds(30));
            }
        }

        public Connector_Specs()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
