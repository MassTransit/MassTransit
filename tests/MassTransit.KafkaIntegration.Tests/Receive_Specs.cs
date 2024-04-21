namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Serializers;
    using TestFramework;
    using Testing;


    public class Receive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "receive";

        [Test]
        public async Task Should_receive()
        {
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
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

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Receive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();
            var harness = provider.GetTestHarness();

            await harness.Start();

            using IProducer<Null, KafkaMessage> p = new ProducerBuilder<Null, KafkaMessage>(new ProducerConfig(provider.GetRequiredService<ClientConfig>()))
                .SetValueSerializer(new MassTransitJsonSerializer<KafkaMessage>())
                .Build();

            var kafkaMessage = new KafkaMessageClass("test");
            var sendContext = new MessageSendContext<KafkaMessage>(kafkaMessage);
            var message = new Message<Null, KafkaMessage>
            {
                Value = kafkaMessage,
                Headers = DictionaryHeadersSerialize.Serializer.Serialize(sendContext)
            };

            await p.ProduceAsync(Topic, message);

            var result = await provider.GetTask<ConsumeContext<KafkaMessage>>();

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(result.Message.Text, Is.EqualTo(message.Value.Text));
                Assert.That(result.MessageId, Is.EqualTo(sendContext.MessageId));
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));

                Assert.That(await harness.Consumed.Any<KafkaMessage>());
            });
        }


        class KafkaMessageClass :
            KafkaMessage
        {
            public KafkaMessageClass(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<KafkaMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
                return Task.CompletedTask;
            }
        }


        public interface KafkaMessage
        {
            string Text { get; }
        }
    }


    public class ConcurrentKeysReceive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "test-concurrent-keys";
        const int NumMessages = 50;
        const int NumKeys = 5;

        [Test]
        public async Task Should_receive_concurrently_by_keys()
        {
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
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
                        rider.AddProducer<int, KafkaMessage>(Topic);

                        rider.UsingKafka((_, _) =>
                        {
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            await harness.Start();

            ITopicProducer<int, KafkaMessage> producer = harness.GetProducer<int, KafkaMessage>();
            for (var i = 0; i < NumMessages; i++)
                await producer.Produce(i % NumKeys, new { Index = i + 1 }, harness.CancellationToken);

            var kafka = provider.GetRequiredService<IKafkaRider>();

            var connected = kafka.ConnectTopicEndpoint<int, KafkaMessage>(Topic, nameof(ConcurrentKeysReceive_Specs), (context, configurator) =>
            {
                configurator.AutoOffsetReset = AutoOffsetReset.Earliest;
                configurator.ConcurrentMessageLimit = NumMessages;

                configurator.ConfigureConsumer<KafkaMessageConsumer>(context);
            });

            await connected.Ready.OrCanceled(harness.CancellationToken);

            await provider.GetTask<ConsumeContext<KafkaMessage>>();

            IList<IReceivedMessage<KafkaMessage>> receivedMessages = await harness.Consumed.SelectAsync<KafkaMessage>().ToListAsync();
            Assert.That(receivedMessages, Has.Count.EqualTo(NumMessages));
            var result = new int[NumKeys];

            foreach (IReceivedMessage<KafkaMessage> receivedMessage in receivedMessages)
            {
                ConsumeContext<KafkaMessage> context = receivedMessage.Context;
                var key = context.GetKey<int>();
                Assert.That(context.Message.Index, Is.GreaterThan(result[key]));
                result[key] = context.Message.Index;
            }
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            static int _index = NumMessages;
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<KafkaMessage> context)
            {
                if (Interlocked.Decrement(ref _index) <= 0)
                    _taskCompletionSource.TrySetResult(context);
                return Task.Delay(10);
            }
        }


        public interface KafkaMessage
        {
            int Index { get; }
        }
    }


    public class ConcurrentConsumersReceive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "multiple-consumers-test";

        [Test]
        public async Task Should_receive_with_multiple_consumers()
        {
            const int concurrentConsumers = 10;
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                    options.Partitions = concurrentConsumers;
                })
                .AddMassTransitTestHarness(x =>
                {
                    for (var i = 0; i < concurrentConsumers; i++)
                        x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();
                    x.AddRider(rider =>
                    {
                        rider.AddProducer<KafkaMessage>(Topic);
                        rider.AddConsumer<KafkaMessageConsumer>();

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(ConcurrentConsumersReceive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConcurrentConsumerLimit = concurrentConsumers / 2;

                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            await harness.Start();

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            await Task.WhenAll(Enumerable.Range(0, concurrentConsumers).Select(i =>
                producer.Produce(new { }, Pipe.Execute<KafkaSendContext>(x => x.Partition = i), harness.CancellationToken)));

            await Task.WhenAll(provider.GetTasks<ConsumeContext<KafkaMessage>>()).OrCanceled(harness.CancellationToken);
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>>[] _taskCompletionSource;

            public KafkaMessageConsumer(IEnumerable<TaskCompletionSource<ConsumeContext<KafkaMessage>>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource.ToArray();
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                var index = (context.Partition() ?? 0) % _taskCompletionSource.Length;
                _taskCompletionSource[index].TrySetResult(context);
            }
        }


        public interface KafkaMessage
        {
        }
    }


    public class BatchReceive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "test-batch";

        [Test]
        public async Task Should_receive_batch()
        {
            const int batchSize = 100;
            var checkpointInterval = TimeSpan.FromMinutes(1);

            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<Batch<KafkaMessage>>>();
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<TestKafkaMessageConsumer<Batch<KafkaMessage>>>(c => c
                            .Options<BatchOptions>(o => o.SetMessageLimit(batchSize)
                                .SetTimeLimit(checkpointInterval)
                                .GroupBy<KafkaMessage, int>(m => m.Partition())));

                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(BatchReceive_Specs), c =>
                            {
                                c.ConfigureConsumer<TestKafkaMessageConsumer<Batch<KafkaMessage>>>(context);

                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConcurrentDeliveryLimit = batchSize;
                                c.CheckpointMessageCount = batchSize;
                                c.CheckpointInterval = checkpointInterval;
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetRequiredService<ITestHarness>();
            await harness.Start();

            var producer = provider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            for (var i = 0; i < batchSize; i++)
                await producer.Produce(new { Index = i }, harness.CancellationToken);

            var result = await provider.GetTask<ConsumeContext<Batch<KafkaMessage>>>();

            for (var i = 0; i < batchSize; i++)
                Assert.That(result.Message[i].Message.Index, Is.EqualTo(i));
        }


        public interface KafkaMessage
        {
            int Index { get; }
        }
    }


    public class MultiGroupReceive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "multi-group";

        [Test]
        public async Task Should_receive_for_multiple_groups()
        {
            var groups = new string[2];
            for (var i = 0; i < groups.Length; i++)
                groups[i] = $"{nameof(MultiGroupReceive_Specs)}_{i + 1}";

            Dictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>> GetTasks(IServiceProvider serviceProvider)
            {
                var harness = serviceProvider.GetTestHarness();
                return groups.ToDictionary(x => x, _ => harness.GetTask<ConsumeContext<KafkaMessage>>());
            }

            await using var provider = new ServiceCollection()
                .AddSingleton(GetTasks)
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<KafkaMessageConsumer>();
                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            foreach (var group in groups)
                            {
                                k.TopicEndpoint<KafkaMessage>(Topic, group, c =>
                                {
                                    c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                    c.ConfigureConsumer<KafkaMessageConsumer>(context);
                                });
                            }
                        });
                    });
                }).BuildServiceProvider();
            var harness = provider.GetTestHarness();

            await harness.Start();
            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();
            await producer.Produce(new { }, harness.CancellationToken);

            var tasks = provider.GetRequiredService<Dictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>>>();

            await Task.WhenAll(tasks.Values.Select(x => x.Task));
        }


        public interface KafkaMessage
        {
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly Dictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>> _taskCompletionSource;

            public KafkaMessageConsumer(Dictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                if (context.TryGetPayload<KafkaConsumeContext>(out var ctx))
                    _taskCompletionSource[ctx.GroupId].TrySetResult(context);
            }
        }
    }
}
