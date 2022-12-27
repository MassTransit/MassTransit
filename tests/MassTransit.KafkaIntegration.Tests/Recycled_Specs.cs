namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class Recycled_Specs :
        InMemoryTestFixture
    {
        const string Topic = "recycle";

        public Recycled_Specs()
        {
            TestTimeout = TimeSpan.FromSeconds(20);
        }

        [Test]
        public async Task Should_produce()
        {
            const int numOfTries = 2;

            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    for (var i = 0; i < numOfTries; i++)
                        x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<KafkaMessageConsumer>();

                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Recycled_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            var busControl = provider.GetRequiredService<IBusControl>();

            //Start all hosted services
            await harness.Start();

            Task<ConsumeContext<KafkaMessage>>[] taskCompletionSources = provider.GetTasks<ConsumeContext<KafkaMessage>>();

            for (var i = 0; i < numOfTries; i++)
            {
                var scope = provider.CreateScope();
                var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

                try
                {
                    await producer.Produce(new { Index = i }, harness.CancellationToken);
                    await taskCompletionSources[i].OrCanceled(harness.CancellationToken);
                }
                finally
                {
                    scope.Dispose();

                    await busControl.StopAsync(harness.CancellationToken);

                    await Task.Delay(100, harness.CancellationToken);

                    await busControl.StartAsync(harness.CancellationToken);
                }
            }
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>>[] _taskCompletionSources;

            public KafkaMessageConsumer(IEnumerable<TaskCompletionSource<ConsumeContext<KafkaMessage>>> taskCompletionSources)
            {
                _taskCompletionSources = taskCompletionSources.ToArray();
            }

            public Task Consume(ConsumeContext<KafkaMessage> context)
            {
                _taskCompletionSources[context.Message.Index].TrySetResult(context);
                return Task.CompletedTask;
            }
        }


        public interface KafkaMessage
        {
            int Index { get; }
        }
    }
}
