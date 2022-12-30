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
    using NUnit.Framework;
    using Serializers;
    using TestFramework;
    using Testing;


    public class Wildcard_Receive_Specs :
        InMemoryTestFixture
    {
        const string TopicPrefix = "Wildcard-topic-";
        static readonly Regex Topic = new($"^{TopicPrefix}[0-9]*", RegexOptions.Compiled);

        [Test]
        public async Task Should_receive_from_multiple_topics_in_wild_card()
        {
            const int numTopics = 2;

            var topicNames = new string[numTopics];
            for (var i = 0; i < numTopics; i++)
                topicNames[i] = TopicPrefix + i;

            Dictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>> CreateTaskCompletionSources(IServiceProvider provider)
            {
                var harness = provider.GetRequiredService<ITestHarness>();
                return topicNames.ToDictionary(x => x, _ => harness.GetTask<ConsumeContext<KafkaMessage>>());
            }

            await using var provider = new ServiceCollection()
                .AddSingleton(CreateTaskCompletionSources)
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = topicNames;
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddRider(r =>
                    {
                        r.AddConsumer<KafkaMessageConsumer>();
                        r.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic.ToString(), nameof(Wildcard_Receive_Specs), c =>
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

            foreach ((var topic, TaskCompletionSource<ConsumeContext<KafkaMessage>> value) in provider
                         .GetRequiredService<Dictionary<string, TaskCompletionSource<ConsumeContext<KafkaMessage>>>>())
            {
                ConsumeContext<KafkaMessage> result = await value.Task;

                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{topic}")));
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
