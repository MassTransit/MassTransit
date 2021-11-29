namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    namespace ConfiguringAzure_Specs
    {
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using Internals;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Messages;
        using Testing;
        using Util;


        [TestFixture]
        public class Configuring_a_bus_instance :
            AsyncTestFixture
        {
            [Test]
            public async Task Should_create_receive_endpoint_and_start()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

                var handled = new TaskCompletionSource<PingMessage>();

                var bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    BusTestFixture.ConfigureBusDiagnostics(x);
                    x.Host(serviceUri, h =>
                    {
                        h.NamedKey(s =>
                        {
                            s.NamedKeyCredential = settings.NamedKeyCredential;
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

                var busHandle = await bus.StartAsync();
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
            public async Task Should_not_create_bus_endpoint_queue_on_startup()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

                var bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    BusTestFixture.ConfigureBusDiagnostics(x);
                    x.Host(serviceUri, h => h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    }));
                });

                var busHandle = await bus.StartAsync();
                try
                {
                }
                finally
                {
                    await busHandle.StopAsync();
                }
            }

            [Test]
            public async Task Should_not_fail_when_slash_is_missing()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                var serviceBusNamespace = Configuration.ServiceNamespace;

                var serviceUri = new Uri($"sb://{serviceBusNamespace}.servicebus.windows.net/Test.Namespace");

                var bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    x.Host(serviceUri, h =>
                    {
                        h.NamedKey(s =>
                        {
                            s.NamedKeyCredential = settings.NamedKeyCredential;
                        });
                    });

                    x.ReceiveEndpoint("test-queue", e =>
                    {
                        // Add a message handler and configure the pipeline to retry the handler
                        // if an exception is thrown
                        e.Handler<A>(Handle, h =>
                        {
                            h.UseRetry(r => r.Interval(5, 100));
                        });
                    });
                });

                var busHandle = await bus.StartAsync();
                try
                {
                }
                finally
                {
                    await busHandle.StopAsync();
                }
            }

            [Test]
            public async Task Should_startup_with_a_receive_endpoint()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

                var bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    BusTestFixture.ConfigureBusDiagnostics(x);
                    x.Host(serviceUri, h => h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    }));

                    x.ReceiveEndpoint("no-messages-allowed", e =>
                    {
                    });
                });

                var busHandle = await bus.StartAsync();
                try
                {
                }
                finally
                {
                    await busHandle.StopAsync();
                }
            }

            [Test]
            public async Task Should_support_the_new_syntax()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();
                var serviceBusNamespace = Configuration.ServiceNamespace;

                var serviceUri = AzureServiceBusEndpointUriCreator.Create(
                    serviceBusNamespace,
                    "MassTransit.Azure.ServiceBus.Core.Tests"
                );

                TaskCompletionSource<A> completed = TaskUtil.GetTask<A>();

                var bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    BusTestFixture.ConfigureBusDiagnostics(x);

                    x.Host(serviceUri, h =>
                    {
                        h.NamedKey(s =>
                        {
                            s.NamedKeyCredential = settings.NamedKeyCredential;
                        });
                    });

                    x.ReceiveEndpoint("input_queue", e =>
                    {
                        e.PrefetchCount = 16;

                        e.Handler<A>(async context => completed.TrySetResult(context.Message));

                        // Add a message handler and configure the pipeline to retry the handler
                        // if an exception is thrown
                        e.Handler<A>(Handle, h =>
                        {
                            h.UseRetry(r => r.Interval(5, 100));
                        });
                    });
                });

                var busHandle = await bus.StartAsync(TestCancellationToken);
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
