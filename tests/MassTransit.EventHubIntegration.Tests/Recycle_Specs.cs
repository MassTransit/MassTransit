namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class Recycled_Specs :
        InMemoryTestFixture
    {
        public Recycled_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }

        [Test]
        public async Task Should_produce()
        {
            const int numOfTries = 2;

            TaskCompletionSource<ConsumeContext<BatchEventHubMessage>>[] taskCompletionSources = Enumerable.Range(0, numOfTries)
                .Select(x => GetTask<ConsumeContext<BatchEventHubMessage>>())
                .ToArray();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSources);

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

                        k.ReceiveEndpoint(Configuration.EventHubName, c =>
                        {
                            c.ConfigureConsumer<EventHubMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            try
            {
                for (var i = 0; i < numOfTries; i++)
                {
                    await busControl.StartAsync(TestCancellationToken);

                    var serviceScope = provider.CreateScope();

                    var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
                    var producer = await producerProvider.GetProducer(Configuration.EventHubName);

                    try
                    {
                        await producer.Produce<BatchEventHubMessage>(new { Index = i }, TestCancellationToken);

                        await taskCompletionSources[i].Task;
                    }
                    finally
                    {
                        serviceScope.Dispose();

                        await busControl.StopAsync(TestCancellationToken);

                        await Task.Delay(100, TestCancellationToken);
                    }
                }
            }
            finally
            {
                await provider.DisposeAsync();
            }
        }


        class EventHubMessageConsumer :
            IConsumer<BatchEventHubMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<BatchEventHubMessage>>[] _taskCompletionSource;

            public EventHubMessageConsumer(TaskCompletionSource<ConsumeContext<BatchEventHubMessage>>[] taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<BatchEventHubMessage> context)
            {
                _taskCompletionSource[context.Message.Index].TrySetResult(context);
            }
        }
    }
}
