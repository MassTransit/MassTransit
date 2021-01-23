namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.Admin;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class ConfigureTopology_Specs :
        InMemoryTestFixture
    {
        const string Host = "localhost:9092";
        const int BrokersCount = 1;

        [Test]
        public async Task Should_create_on_start()
        {
            var services = new ServiceCollection();

            const ushort partitionCount = 2;
            const short replicaCount = BrokersCount;
            const string topicName = "create-topic";

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.UsingKafka((context, k) =>
                    {
                        k.Host(Host);

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
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var config = new AdminClientConfig {BootstrapServers = Host};
            var client = new AdminClientBuilder(config).Build();

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                var meta = client.GetMetadata(topicName, TimeSpan.FromSeconds(10));

                Assert.AreEqual(1, meta.Topics.Count);

                foreach (var topic in meta.Topics)
                {
                    Assert.AreEqual(partitionCount, topic.Partitions.Count);

                    foreach (var partition in topic.Partitions)
                    {
                        Assert.AreEqual(replicaCount, partition.Replicas.Length);
                    }
                }

                await Task.Delay(1000);
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();

                try
                {
                    await client.DeleteTopicsAsync(new[] {topicName});
                }
                catch (DeleteTopicsException)
                {
                    //suppress
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        [Test]
        public async Task Should_bypass_if_created()
        {
            var services = new ServiceCollection();

            const ushort partitionCount = 2;
            const short replicaCount = BrokersCount;
            const string topicName = "do-not-create-topic";
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddSingleton(taskCompletionSource);

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.AddProducer<KafkaMessage>(topicName);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host(Host);

                        k.TopicEndpoint<KafkaMessage>(topicName, nameof(ConfigureTopology_Specs), c =>
                        {
                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);

                            c.CreateIfMissing(t =>
                            {
                                t.NumPartitions = partitionCount;
                                t.ReplicationFactor = replicaCount;
                            });
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var config = new AdminClientConfig {BootstrapServers = Host};
            var client = new AdminClientBuilder(config).Build();
            var specification = new TopicSpecification
            {
                Name = topicName,
                NumPartitions = partitionCount,
                ReplicationFactor = replicaCount
            };
            await client.CreateTopicsAsync(new[] {specification});

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producer = serviceScope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            try
            {
                await producer.Produce(new {Text = "text"}, TestCancellationToken);

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                Assert.NotNull(result);
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();

                try
                {
                    await client.DeleteTopicsAsync(new[] {topicName});
                }
                catch (DeleteTopicsException)
                {
                    //suppress
                }
                finally
                {
                    client.Dispose();
                }
            }
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
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface KafkaMessage
        {
        }
    }
}
