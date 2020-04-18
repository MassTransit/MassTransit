namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    namespace ConfiguringAzure_Specs
    {
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using GreenPipes;
        using GreenPipes.Internals.Extensions;
        using Hosting;
        using MassTransit.Testing;
        using Microsoft.Azure.ServiceBus;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Messages;
        using Util;


        [TestFixture]
        public class Configuring_a_bus_instance :
            AsyncTestFixture
        {
            [Test]
            public async Task Should_support_the_new_syntax()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();
                var serviceBusNamespace = Configuration.ServiceNamespace;

                Uri serviceUri = AzureServiceBusEndpointUriCreator.Create(
                    serviceBusNamespace,
                    "MassTransit.Azure.ServiceBus.Core.Tests"
                );

                var completed = TaskUtil.GetTask<A>();

                IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    IServiceBusHost host = x.Host(serviceUri, h =>
                    {
                        h.SharedAccessSignature(s =>
                        {
                            s.KeyName = settings.KeyName;
                            s.SharedAccessKey = settings.SharedAccessKey;
                            s.TokenTimeToLive = settings.TokenTimeToLive;
                            s.TokenScope = settings.TokenScope;
                        });
                    });

                    x.ReceiveEndpoint(host, "input_queue", e =>
                    {
                        e.PrefetchCount = 16;

                        e.UseExecute(context => Console.WriteLine(
                            $"Received (input_queue): {context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A")}, Types = ({string.Join(",", context.SupportedMessageTypes)})"));

                        e.Handler<A>(async context => completed.TrySetResult(context.Message));

                        // Add a message handler and configure the pipeline to retry the handler
                        // if an exception is thrown
                        e.Handler<A>(Handle, h =>
                        {
                            h.UseRetry(r => r.Interval(5, 100));
                        });
                    });
                });

                BusHandle busHandle = await bus.StartAsync();
                try
                {
                }
                finally
                {
                    await busHandle.StopAsync();
                }

                //                }))
                //                {
                //                    var queueAddress = new Uri(hostAddress, "input_queue");
                //                    ISendEndpoint endpoint = bus.GetSendEndpoint(queueAddress);
                //
                //                    await endpoint.Send(new A());
                //                }
            }

            [Test]
            public async Task Should_not_create_bus_endpoint_queue_on_startup()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                Uri serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

                IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    BusTestFixture.ConfigureBusDiagnostics(x);
                    x.Host(serviceUri, h => h.SharedAccessSignature(s =>
                    {
                        s.KeyName = settings.KeyName;
                        s.SharedAccessKey = settings.SharedAccessKey;
                        s.TokenTimeToLive = settings.TokenTimeToLive;
                        s.TokenScope = settings.TokenScope;
                    }));
                });

                BusHandle busHandle = await bus.StartAsync();
                try
                {
                }
                finally
                {
                    await busHandle.StopAsync();
                }
            }

            [Test]
            public async Task Should_create_receive_endpoint_and_start()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                Uri serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

                TaskCompletionSource<PingMessage> handled = new TaskCompletionSource<PingMessage>();

                IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    BusTestFixture.ConfigureBusDiagnostics(x);
                    x.Host(serviceUri, h =>
                    {
                    #if NET461
                        h.TransportType = TransportType.AmqpWebSockets;
                    #endif
                        h.SharedAccessSignature(s =>
                        {
                            s.KeyName = settings.KeyName;
                            s.SharedAccessKey = settings.SharedAccessKey;
                            s.TokenTimeToLive = settings.TokenTimeToLive;
                            s.TokenScope = settings.TokenScope;
                        });
                    });

                    x.ReceiveEndpoint("test-input-queue", e =>
                    {
                        e.Handler<PingMessage>(async context =>
                        {
                            handled.TrySetResult(context.Message);
                        });
                    });
                });

                BusHandle busHandle = await bus.StartAsync();
                try
                {
                    var endpoint = await bus.GetSendEndpoint(new Uri("queue:test-input-queue"));
                    await endpoint.Send(new PingMessage());

                    await handled.Task.OrTimeout(TimeSpan.FromSeconds(10000));
                }
                finally
                {
                    await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
                }
            }

            [Test]
            public async Task Should_not_fail_when_slash_is_missing()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                var serviceBusNamespace = Configuration.ServiceNamespace;

                Uri serviceUri = new Uri($"sb://{serviceBusNamespace}.servicebus.windows.net/Test.Namespace");

                IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    IServiceBusHost host = x.Host(serviceUri, h =>
                    {
                        h.SharedAccessSignature(s =>
                        {
                            s.KeyName = settings.KeyName;
                            s.SharedAccessKey = settings.SharedAccessKey;
                            s.TokenTimeToLive = settings.TokenTimeToLive;
                            s.TokenScope = settings.TokenScope;
                        });
                    });

                    x.ReceiveEndpoint(host, "test-queue", e =>
                    {
                        // Add a message handler and configure the pipeline to retry the handler
                        // if an exception is thrown
                        e.Handler<A>(Handle, h =>
                        {
                            h.UseRetry(r => r.Interval(5, 100));
                        });
                    });
                });

                BusHandle busHandle = await bus.StartAsync();
                try
                {
                }
                finally
                {
                    await busHandle.StopAsync();
                }
            }

            public Configuring_a_bus_instance()
                : base(new InMemoryTestHarness())
            {
            }

            async Task Handle(ConsumeContext<A> context)
            {
            }


            class A
            {
            }
        }
    }
}
