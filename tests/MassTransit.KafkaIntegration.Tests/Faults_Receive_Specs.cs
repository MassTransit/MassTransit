namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Serializers;
    using TestFramework;
    using Testing;


    public class Faults_Receive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "failure-receive-test";

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
                        rider.AddProducer<KafkaMessage>(Topic);

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

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            await producer.Produce(new { Index = 0 }, harness.CancellationToken);
            await producer.Produce(new { Index = 1 }, harness.CancellationToken);

            await provider.GetTask<ConsumeContext<KafkaMessage>>();

            Assert.That(await harness.Consumed.Any<KafkaMessage>(x => x.Exception != null));
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
                if (context.Message.Index == 0)
                    throw new ArgumentException("Expected failure");

                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface KafkaMessage
        {
            int Index { get; }
        }
    }


    public class PoisonPill_Receive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "poison-pill-receive-test";

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
                        rider.AddConsumer<TestKafkaMessageConsumer<KafkaMessage>>();
                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(PoisonPill_Receive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<TestKafkaMessageConsumer<KafkaMessage>>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            await harness.Start();

            using IProducer<Null, PoisonPill> p = new ProducerBuilder<Null, PoisonPill>(new ProducerConfig(provider.GetRequiredService<ClientConfig>()))
                .SetValueSerializer(new MassTransitJsonSerializer<PoisonPill>())
                .Build();

            var kafkaMessage = new PoisonPill(1);
            var sendContext = new MessageSendContext<PoisonPill>(kafkaMessage);
            var message = new Message<Null, PoisonPill>
            {
                Value = kafkaMessage,
                Headers = DictionaryHeadersSerialize.Serializer.Serialize(sendContext)
            };

            await p.ProduceAsync(Topic, message);

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            await producer.Produce(new { Id = NewId.NextGuid() }, harness.CancellationToken);

            await provider.GetTask<ConsumeContext<KafkaMessage>>();
        }


        public interface KafkaMessage
        {
            Guid Id { get; }
        }


        class PoisonPill
        {
            public PoisonPill(int id)
            {
                Id = id;
            }

            public int Id { get; set; }
        }
    }
}
