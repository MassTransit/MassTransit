namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using DependencyInjection;
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
            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

            var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

            var provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddMassTransit(x =>
                {
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(serviceUri, h =>
                        {
                            h.NamedKey(s =>
                            {
                                s.NamedKeyCredential = settings.NamedKeyCredential;
                            });
                        });
                    });
                }).BuildServiceProvider(true);

            var depot = provider.GetRequiredService<IBusDepot>();

            await depot.Start(TestCancellationToken);
            try
            {
                var bus = provider.GetRequiredService<IBus>();

                var connector = provider.GetRequiredService<ISubscriptionEndpointConnector>();

                Task<ConsumeContext<PingMessage>> handled = null;
                var handle = connector.ConnectSubscriptionEndpoint<PingMessage>("my-sub", e =>
                {
                    handled = Handled<PingMessage>(e);
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
        public async Task Should_connect_subscription_endpoint_on_the_second_bus()
        {
            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

            var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

            var provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddMassTransit(x => x.UsingInMemory())
                .AddMassTransit<IDeuceBus>(x =>
                {
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(serviceUri, h =>
                        {
                            h.NamedKey(s =>
                            {
                                s.NamedKeyCredential = settings.NamedKeyCredential;
                            });
                        });
                    });
                }).BuildServiceProvider(true);

            var depot = provider.GetRequiredService<IBusDepot>();

            await depot.Start(TestCancellationToken);
            try
            {
                var bus = provider.GetRequiredService<IDeuceBus>();

                var connector = provider.GetRequiredService<ISubscriptionEndpointConnector>();

                Task<ConsumeContext<PingMessage>> handled = null;
                var handle = connector.ConnectSubscriptionEndpoint<PingMessage>("my-sub", e =>
                {
                    handled = Handled<PingMessage>(e);
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
        public async Task Should_connect_subscription_endpoint_on_the_second_bus_explicitly()
        {
            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

            var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

            var provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddMassTransit(x => x.UsingInMemory())
                .AddMassTransit<IDeuceBus>(x =>
                {
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(serviceUri, h =>
                        {
                            h.NamedKey(s =>
                            {
                                s.NamedKeyCredential = settings.NamedKeyCredential;
                            });
                        });
                    });
                }).BuildServiceProvider(true);

            var depot = provider.GetRequiredService<IBusDepot>();

            await depot.Start(TestCancellationToken);
            try
            {
                var bus = provider.GetRequiredService<IDeuceBus>();

                var connector = provider.GetRequiredService<Bind<IDeuceBus, ISubscriptionEndpointConnector>>().Value;

                Task<ConsumeContext<PingMessage>> handled = null;
                var handle = connector.ConnectSubscriptionEndpoint<PingMessage>("my-sub", e =>
                {
                    handled = Handled<PingMessage>(e);
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


        public interface IDeuceBus :
            IBus
        {
        }


        public Connector_Specs()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
