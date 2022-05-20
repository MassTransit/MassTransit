namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DependencyInjection;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class MultiBus_Specs :
        InMemoryTestFixture
    {
        public MultiBus_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }

        [Test]
        public async Task Should_receive_in_both_buses()
        {
            await using var provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddSingleton(GetTask<ConsumeContext<FirstBusMessage>>())
                .AddSingleton(GetTask<ConsumeContext<SecondBusMessage>>())
                .AddMassTransit<IFirstBus>(x =>
                {
                    x.AddRider(r =>
                    {
                        r.AddConsumer<FirstBusMessageConsumer>();

                        r.UsingEventHub((context, k) =>
                        {
                            k.Host(Configuration.EventHubNamespace);
                            k.Storage(Configuration.StorageAccount);

                            k.ReceiveEndpoint(Configuration.EventHubName, c =>
                            {
                                c.ConfigureConsumer<FirstBusMessageConsumer>(context);
                            });
                        });
                    });

                    x.UsingInMemory();
                })
                .AddMassTransit<ISecondBus>(x =>
                {
                    x.AddRider(r =>
                    {
                        r.AddConsumer<SecondBusMessageConsumer>();

                        r.UsingEventHub((context, k) =>
                        {
                            k.Host(Configuration.EventHubNamespace);
                            k.Storage(Configuration.StorageAccount);

                            k.ReceiveEndpoint(Configuration.EventHubName, c =>
                            {
                                c.ConfigureConsumer<SecondBusMessageConsumer>(context);
                            });
                        });
                    });

                    x.UsingInMemory();
                })
                .BuildServiceProvider(true);

            IEnumerable<IHostedService> hostedServices = provider.GetServices<IHostedService>().ToArray();

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));

            var serviceScope = provider.CreateScope();

            var producerProvider = serviceScope.ServiceProvider.GetRequiredService<Bind<IFirstBus, IEventHubProducerProvider>>().Value;
            var producer = await producerProvider.GetProducer(Configuration.EventHubName);

            await producer.Produce(new FirstBusMessage(), TestCancellationToken);

            await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<FirstBusMessage>>>().Task.OrCanceled(TestCancellationToken);

            await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<SecondBusMessage>>>().Task.OrCanceled(TestCancellationToken);

            serviceScope.Dispose();

            await Task.WhenAll(hostedServices.Select(x => x.StopAsync(TestCancellationToken)));
        }


        public interface ISecondBus :
            IBus
        {
        }


        public interface IFirstBus :
            IBus
        {
        }


        class FirstBusMessage
        {
        }


        class SecondBusMessage
        {
        }


        class FirstBusMessageConsumer :
            IConsumer<FirstBusMessage>
        {
            readonly IEventHubProducerProvider _provider;
            readonly TaskCompletionSource<ConsumeContext<FirstBusMessage>> _taskCompletionSource;

            public FirstBusMessageConsumer(TaskCompletionSource<ConsumeContext<FirstBusMessage>> taskCompletionSource,
                Bind<ISecondBus, IEventHubProducerProvider> providerBind)
            {
                _taskCompletionSource = taskCompletionSource;
                _provider = providerBind.Value;
            }

            public async Task Consume(ConsumeContext<FirstBusMessage> context)
            {
                var producer = await _provider.GetProducer(Configuration.EventHubName);
                await producer.Produce(new SecondBusMessage());
                _taskCompletionSource.TrySetResult(context);
            }
        }


        class SecondBusMessageConsumer :
            IConsumer<SecondBusMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<SecondBusMessage>> _taskCompletionSource;

            public SecondBusMessageConsumer(TaskCompletionSource<ConsumeContext<SecondBusMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<SecondBusMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }
    }
}
