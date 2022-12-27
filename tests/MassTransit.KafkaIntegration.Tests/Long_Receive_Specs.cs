namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class Long_Receive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "long-receive-test";

        [Test]
        public async Task Should_receive()
        {
            var services = new ServiceCollection();

            services.AddOptions<TestHarnessOptions>()
                .Configure(x =>
                {
                    x.TestInactivityTimeout = TimeSpan.FromSeconds(10);
                    x.TestTimeout = TimeSpan.FromMinutes(Debugger.IsAttached ? 50 : 2);
                });
            await using var provider = services.ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<KafkaMessageConsumer>();
                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Long_Receive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConcurrentMessageLimit = 1;
                                c.CheckpointMessageCount = 1;
                                c.CheckpointInterval = TimeSpan.FromSeconds(1);
                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();

            await harness.Start();

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            await producer.Produce(new { }, harness.CancellationToken);

            await provider.GetTask<ConsumeContext<KafkaMessage>>();
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface KafkaMessage
        {
        }
    }
}
