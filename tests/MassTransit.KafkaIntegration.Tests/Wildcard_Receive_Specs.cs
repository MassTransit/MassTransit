namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serializers;
    using TestFramework;


    public class Wildcard_Receive_Specs :
        InMemoryTestFixture
    {
        const string Host = "localhost:9092";
        const string TopicPrefix = "Wildcard-topic-";
        static readonly Regex Topic = new($"^{TopicPrefix}[0-9]*", RegexOptions.Compiled);

        [Test]
        public async Task Should_receive()
        {
            const int numTopics = 2;

            var topicNames = new string[numTopics];
            for (var i = 0; i < numTopics; i++)
                topicNames[i] = TopicPrefix + i;

            Dictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>> taskCompletionSource =
                topicNames.ToDictionary(x => x, _ => GetTask<ConsumeContext<KafkaMessage>>());
            var clientConfig = new ClientConfig { BootstrapServers = Host };

            await using var provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(LoggerFactory)
                .AddSingleton(taskCompletionSource)
                .AddSingleton(new TopicCreator(clientConfig))
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddMassTransit(x =>
                {
                    x.AddRider(r =>
                    {
                        r.AddConsumer<KafkaMessageConsumer>();

                        r.UsingKafka((context, k) =>
                        {
                            k.Host(Host);

                            k.TopicEndpoint<KafkaMessage>(Topic.ToString(), nameof(Wildcard_Receive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });

                    x.UsingInMemory();
                }).BuildServiceProvider();

            var topicCreator = provider.GetRequiredService<TopicCreator>();

            var busControl = provider.GetRequiredService<IBusControl>();

            try
            {
                await topicCreator.CreateTopics(2, 1, topicNames);

                await busControl.StartAsync(TestCancellationToken);

                using IProducer<Null, KafkaMessage> p = new ProducerBuilder<Null, KafkaMessage>(new ProducerConfig(clientConfig))
                    .SetKeySerializer(Serializers.Null)
                    .SetValueSerializer(new MassTransitJsonSerializer<KafkaMessage>())
                    .Build();

                var kafkaMessage = new KafkaMessageClass("test");
                var sendContext = new MessageSendContext<KafkaMessage>(kafkaMessage);
                var message = new Message<Null, KafkaMessage>
                {
                    Value = kafkaMessage,
                    Headers = DictionaryHeadersSerialize.Serializer.Serialize(sendContext)
                };

                foreach (var topicName in topicNames)
                    await p.ProduceAsync(topicName, message);

                p.Flush();
                p.Dispose();

                foreach ((var topic, TaskCompletionSource<ConsumeContext<KafkaMessage>> value) in taskCompletionSource)
                {
                    ConsumeContext<KafkaMessage> result = await value.Task;

                    Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{topic}")));
                }
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await topicCreator.DisposeAsync();

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
            readonly ConcurrentDictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>> _taskCompletionSource;

            public KafkaMessageConsumer(Dictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>> taskCompletionSource)
            {
                _taskCompletionSource = new ConcurrentDictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>>(taskCompletionSource);
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                var payload = context.GetPayload<KafkaConsumeContext<Ignore>>();
                if (_taskCompletionSource.TryGetValue(payload.Topic, out TaskCompletionSource<ConsumeContext<KafkaMessage>> source))
                    source.TrySetResult(context);
            }
        }


        public interface KafkaMessage
        {
        }
    }
}
