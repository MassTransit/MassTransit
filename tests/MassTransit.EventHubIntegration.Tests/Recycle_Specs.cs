namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class Recycled_Specs :
        InMemoryTestFixture
    {
        const string EventHubName = "default-eh";

        public Recycled_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(1);
        }

        [Test]
        public async Task Should_produce()
        {
            TaskCompletionSource<ConsumeContext<BatchEventHubMessage>> taskCompletionSource = GetTask<ConsumeContext<BatchEventHubMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<EventHubMessageConsumer>();

                    rider.UsingEventHub((context, k) =>
                    {
                        k.Host(Configuration.EventHubNamespace);
                        k.Storage(Configuration.StorageAccount);

                        k.ReceiveEndpoint(EventHubName, Configuration.ConsumerGroup, c =>
                        {
                            c.ConfigureConsumer<EventHubMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();
            await busControl.StartAsync(TestCancellationToken);
            await busControl.StopAsync(TestCancellationToken);

            await Task.Delay(500, TestCancellationToken);

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateAsyncScope();
            try
            {
                var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
                var producer = await producerProvider.GetProducer(EventHubName);
                await producer.Produce<BatchEventHubMessage>(new { }, TestCancellationToken);
                await taskCompletionSource.Task.OrCanceled(TestCancellationToken);

                await busControl.StopAsync(TestCancellationToken);
            }
            finally
            {
                await serviceScope.DisposeAsync();
                await provider.DisposeAsync();
            }
        }


        class EventHubMessageConsumer :
            IConsumer<BatchEventHubMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<BatchEventHubMessage>> _taskCompletionSource;

            public EventHubMessageConsumer(TaskCompletionSource<ConsumeContext<BatchEventHubMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<BatchEventHubMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }
    }


    public class Recycled_Produce_Specs :
        InMemoryTestFixture
    {
        const string EventHubName = "default-eh";

        [Test]
        public async Task Should_produce_after_recycle()
        {
            var services = new ServiceCollection();

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.UsingEventHub((_, k) =>
                    {
                        k.Host(Configuration.EventHubNamespace);
                        k.Storage(Configuration.StorageAccount);

                        k.ReceiveEndpoint(EventHubName, Configuration.ConsumerGroup,
                            c => c.Handler<BatchEventHubMessage>(_ => Task.CompletedTask));
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();
            try
            {
                await busControl.StartAsync(TestCancellationToken);
                using (var scope = provider.CreateAsyncScope())
                {
                    var producerProvider = scope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
                    var producer = await producerProvider.GetProducer(EventHubName);
                    await producer.Produce<BatchEventHubMessage>(new { }, TestCancellationToken);
                }

                await busControl.StopAsync(TestCancellationToken);

                await Task.Delay(500, TestCancellationToken);

                await busControl.StartAsync(TestCancellationToken);

                await using (var scope = provider.CreateAsyncScope())
                {
                    var producerProvider = scope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
                    var producer = await producerProvider.GetProducer(EventHubName);
                    await producer.Produce<BatchEventHubMessage>(new { }, TestCancellationToken);
                }

                await busControl.StopAsync(TestCancellationToken);
            }
            finally
            {
                await provider.DisposeAsync();
            }
        }
    }
}
