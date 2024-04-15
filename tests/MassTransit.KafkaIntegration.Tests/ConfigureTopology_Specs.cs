namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class ConfigureTopology_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_create_on_start()
        {
            const ushort partitionCount = 2;
            const short replicaCount = 1;
            const string topicName = "create-topic";
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddRider(rider =>
                    {
                        rider.UsingKafka((_, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(topicName, nameof(ConfigureTopology_Specs), c =>
                            {
                                c.CreateIfMissing(t =>
                                {
                                    t.NumPartitions = partitionCount;
                                    t.ReplicationFactor = replicaCount;
                                });
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            await harness.Start();

            var client = provider.GetRequiredService<IAdminClient>();

            var meta = client.GetMetadata(topicName, TimeSpan.FromSeconds(10));

            Assert.That(meta.Topics.Count, Is.EqualTo(1));

            foreach (var topic in meta.Topics)
            {
                Assert.That(topic.Partitions.Count, Is.EqualTo(partitionCount));

                foreach (var partition in topic.Partitions)
                    Assert.That(partition.Replicas.Length, Is.EqualTo(replicaCount));
            }
        }

        [Test]
        public async Task Should_bypass_if_created()
        {
            const ushort partitionCount = 2;
            const short replicaCount = 1;
            const string topicName = "do-not-create-topic";
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { topicName };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<TestKafkaMessageConsumer<KafkaMessage>>();

                        rider.AddProducer<KafkaMessage>(topicName);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(topicName, nameof(ConfigureTopology_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConfigureConsumer<TestKafkaMessageConsumer<KafkaMessage>>(context);

                                c.CreateIfMissing(t =>
                                {
                                    t.NumPartitions = partitionCount;
                                    t.ReplicationFactor = replicaCount;
                                });
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            await harness.Start();

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            await producer.Produce(new { Text = "text" }, harness.CancellationToken);

            var result = await provider.GetTask<ConsumeContext<KafkaMessage>>();

            Assert.NotNull(result);
        }


        public interface KafkaMessage
        {
        }
    }
}
