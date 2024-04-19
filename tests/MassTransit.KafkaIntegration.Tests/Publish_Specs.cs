namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class Publish_Specs :
        InMemoryTestFixture
    {
        const string Topic = "publish";

        [Test]
        public async Task Should_receive_in_kafka_and_in_bus()
        {
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<BusPingConsumer>();

                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();
                    x.AddTaskCompletionSource<ConsumeContext<BusPing>>();
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<KafkaMessageConsumer>();
                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Publish_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            await harness.Start();

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();
            await producer.Produce(new { Text = "test" }).OrCanceled(harness.CancellationToken);

            var result = await provider.GetTask<ConsumeContext<KafkaMessage>>();
            var ping = await provider.GetTask<ConsumeContext<BusPing>>();

            Assert.Multiple(() =>
            {
                Assert.That(ping.InitiatorId, Is.EqualTo(result.CorrelationId));

                Assert.That(ping.SourceAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));
            });
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly IPublishEndpoint _publishEndpoint;
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(IPublishEndpoint publishEndpoint, TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _publishEndpoint = publishEndpoint;
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
                await _publishEndpoint.Publish<BusPing>(new { });
            }
        }


        class BusPingConsumer :
            IConsumer<BusPing>
        {
            readonly TaskCompletionSource<ConsumeContext<BusPing>> _taskCompletionSource;

            public BusPingConsumer(TaskCompletionSource<ConsumeContext<BusPing>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<BusPing> context)
            {
                _taskCompletionSource.TrySetResult(context);
                return Task.CompletedTask;
            }
        }


        public interface KafkaMessage
        {
            string Text { get; }
        }


        public interface BusPing
        {
        }
    }
}
