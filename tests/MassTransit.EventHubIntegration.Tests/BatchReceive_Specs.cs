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


    public class BatchReceive_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_batch()
        {
            const int batchSize = 100;
            var checkpointInterval = TimeSpan.FromMinutes(1);

            TaskCompletionSource<ConsumeContext<Batch<BatchEventHubMessage>>> taskCompletionSource = GetTask<ConsumeContext<Batch<BatchEventHubMessage>>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<EventHubMessageConsumer>(c => c
                        .Options<BatchOptions>(o => o.SetMessageLimit(batchSize)
                            .SetTimeLimit(checkpointInterval)
                            .GroupBy<BatchEventHubMessage, string>(m => m.PartitionKey())));

                    rider.UsingEventHub((context, k) =>
                    {
                        k.Host(Configuration.EventHubNamespace);
                        k.Storage(Configuration.StorageAccount);

                        k.ReceiveEndpoint(Configuration.EventHubName, c =>
                        {
                            c.ConfigureConsumer<EventHubMessageConsumer>(context);

                            c.CheckpointMessageCount = batchSize;
                            c.CheckpointInterval = checkpointInterval;
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
            var producer = await producerProvider.GetProducer(Configuration.EventHubName);

            try
            {
                var messages = Enumerable.Range(0, batchSize).Select(x => new { Index = x }).ToArray();
                await producer.Produce<BatchEventHubMessage>(messages, TestCancellationToken);

                ConsumeContext<Batch<BatchEventHubMessage>> result = await taskCompletionSource.Task;

                Assert.AreEqual(batchSize, result.Message.Length);

                for (var i = 0; i < batchSize; i++)
                    Assert.AreEqual(i, result.Message[i].Message.Index);
            }
            finally
            {
                serviceScope.Dispose();

                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        class EventHubMessageConsumer :
            IConsumer<Batch<BatchEventHubMessage>>
        {
            readonly TaskCompletionSource<ConsumeContext<Batch<BatchEventHubMessage>>> _taskCompletionSource;

            public EventHubMessageConsumer(TaskCompletionSource<ConsumeContext<Batch<BatchEventHubMessage>>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<Batch<BatchEventHubMessage>> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }
    }
}
