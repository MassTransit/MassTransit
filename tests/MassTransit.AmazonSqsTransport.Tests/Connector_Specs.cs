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

        [Test]
        public async Task Should_disconnect_connected_endpoint()
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
                var connector = provider.GetRequiredService<IReceiveEndpointConnector>();
                var endpointNameFormatter = provider.GetService<IEndpointNameFormatter>() ?? DefaultEndpointNameFormatter.Instance;
                var definition = new TemporaryEndpointDefinition();
                var handle = connector.ConnectReceiveEndpoint(definition, endpointNameFormatter);

                await handle.Ready;

                var disconnected = await connector.DisconnectReceiveEndpoint(definition, endpointNameFormatter);

                Assert.AreEqual(disconnected, true);
            }
            finally
            {
                await depot.Stop(TimeSpan.FromSeconds(30));
            }
        }

        [Test]
        public async Task Should_throw_ConfigurationException_when_endpoint_is_not_exist()
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
                var connector = provider.GetRequiredService<IReceiveEndpointConnector>();
                var endpointNameFormatter = provider.GetService<IEndpointNameFormatter>() ?? DefaultEndpointNameFormatter.Instance;
                var handle = connector.ConnectReceiveEndpoint(new TemporaryEndpointDefinition(), endpointNameFormatter);

                await handle.Ready;

                Assert.ThrowsAsync<ConfigurationException>(() => connector.DisconnectReceiveEndpoint(new TemporaryEndpointDefinition(), endpointNameFormatter));
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
