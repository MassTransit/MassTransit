namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    namespace ConfiguringAzure_Specs
    {
        using System;
        using System.Threading.Tasks;
        using GreenPipes;
        using Hosting;
        using MassTransit.Testing;
        using NUnit.Framework;
        using TestFramework;
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
