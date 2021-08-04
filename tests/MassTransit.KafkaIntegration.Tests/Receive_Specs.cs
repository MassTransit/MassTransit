namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serializers;
    using TestFramework;


    public class Receive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "test";
        const string TopicConcurrent = "test-concurrent";

        [Test]
        public async Task Should_receive()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(Receive_Specs), c =>
                        {
                            c.CreateIfMissing();
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var observer = GetConsumeObserver();
            busControl.ConnectConsumeObserver(observer);

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                var config = new ProducerConfig {BootstrapServers = "localhost:9092"};

                using IProducer<Null, KafkaMessage> p = new ProducerBuilder<Null, KafkaMessage>(config)
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

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual(message.Value.Text, result.Message.Text);
                Assert.AreEqual(sendContext.MessageId, result.MessageId);
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));

                Assert.That(await observer.Messages.Any<KafkaMessage>());
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }

        [Test]
        public async Task Should_receive_concurrently()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(TopicConcurrent, nameof(Receive_Specs), c =>
                        {
                            c.CreateIfMissing();
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);

                            c.CheckpointMessageCount = 10;
                            c.ConcurrentMessageLimit = 100;
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var observer = GetConsumeObserver();
            busControl.ConnectConsumeObserver(observer);

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                var config = new ProducerConfig {BootstrapServers = "localhost:9092"};

                using IProducer<Null, KafkaMessage> p = new ProducerBuilder<Null, KafkaMessage>(config)
                    .SetValueSerializer(new MassTransitJsonSerializer<KafkaMessage>())
                    .Build();

                var kafkaMessage = new KafkaMessageClass("test");
                var sendContext = new MessageSendContext<KafkaMessage>(kafkaMessage);
                var message = new Message<Null, KafkaMessage>
                {
                    Value = kafkaMessage,
                    Headers = DictionaryHeadersSerialize.Serializer.Serialize(sendContext)
                };

                await Task.WhenAll(Enumerable.Range(0, 100).Select(x => p.ProduceAsync(TopicConcurrent, message)));

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual(message.Value.Text, result.Message.Text);
                Assert.AreEqual(sendContext.MessageId, result.MessageId);
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{TopicConcurrent}")));

                Assert.That(await observer.Messages.Any<KafkaMessage>());
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
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

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface KafkaMessage
        {
            string Text { get; }
        }
    }


    public class BatchReceive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "test-batch";

        [Test]
        public async Task Should_receive_batch()
        {
            TaskCompletionSource<ConsumeContext<Batch<KafkaMessage>>> taskCompletionSource = GetTask<ConsumeContext<Batch<KafkaMessage>>>();

            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            const int batchSize = 100;
            var checkpointInterval = TimeSpan.FromMinutes(1);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>(c => c
                        .Options<BatchOptions>(o => o.SetMessageLimit(batchSize)
                            .SetTimeLimit(checkpointInterval)
                            .GroupBy<KafkaMessage, int>(m => m.Partition())));

                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(BatchReceive_Specs), c =>
                        {
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            c.CreateIfMissing();

                            c.CheckpointMessageCount = batchSize;
                            c.CheckpointInterval = checkpointInterval;
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                var producer = provider.GetRequiredService<ITopicProducer<KafkaMessage>>();

                for (var i = 0; i < batchSize; i++)
                    await producer.Produce(new {Index = i}, TestCancellationToken);

                ConsumeContext<Batch<KafkaMessage>> result = await taskCompletionSource.Task;

                Assert.AreEqual(batchSize, result.Message.Length);

                for (var i = 0; i < batchSize; i++)
                    Assert.AreEqual(i, result.Message[i].Message.Index);
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        class KafkaMessageConsumer :
            IConsumer<Batch<KafkaMessage>>
        {
            readonly TaskCompletionSource<ConsumeContext<Batch<KafkaMessage>>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<Batch<KafkaMessage>>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<Batch<KafkaMessage>> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface KafkaMessage
        {
            int Index { get; }
        }
    }


    public class ReceiveWithPayload_Specs :
        InMemoryTestFixture
    {
        const string Topic = "test-payload";

        [Test]
        public async Task Should_contains_payload()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();
                    rider.AddProducer<string, KafkaMessage>(Topic, (context, c) => c.SetKeySerializer(Serializers.Utf8));

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<string, KafkaMessage>(Topic, nameof(ReceiveWithPayload_Specs), c =>
                        {
                            c.CreateIfMissing();
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);

                            c.SetKeyDeserializer(Deserializers.Utf8);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var observer = GetConsumeObserver();
            busControl.ConnectConsumeObserver(observer);

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                var producer = provider.GetRequiredService<ITopicProducer<string, KafkaMessage>>();
                var key = NewId.NextGuid().ToString();
                await producer.Produce(key, new { }, TestCancellationToken);

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                Assert.IsTrue(result.TryGetPayload(out KafkaConsumeContext<string> _));
                Assert.AreEqual(key, result.GetKey<string>());
                Assert.That(await observer.Messages.Any<KafkaMessage>());
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
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
