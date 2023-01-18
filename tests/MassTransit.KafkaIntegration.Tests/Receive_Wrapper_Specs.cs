namespace MassTransit.KafkaIntegration.Tests;

using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Context;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Serializers;
using TestFramework;
using Testing;


public class Receive_Wrapper_Specs :
        InMemoryTestFixture
    {
        const string Topic = "receive_wrapper";

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
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessageWithProducerWrapper>>();
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<KafkaMessageWrapperConsumer>();

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessageWithProducerWrapper>(Topic, nameof(Receive_Wrapper_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<KafkaMessageWrapperConsumer>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();
            var harness = provider.GetTestHarness();

            await harness.Start();

            using IProducer<Null, KafkaMessageWithProducerWrapper> p = new ProducerBuilder<Null, KafkaMessageWithProducerWrapper>(new ProducerConfig(provider.GetRequiredService<ClientConfig>()))
                .SetValueSerializer(new MassTransitJsonSerializer<KafkaMessageWithProducerWrapper>())
                .Build();

            var kafkaMessage = new KafkaMessageWithProducerWrapperClass("test");
            var sendContext = new MessageSendContext<KafkaMessageWithProducerWrapper>(kafkaMessage);
            var message = new Message<Null, KafkaMessageWithProducerWrapper>
            {
                Value = kafkaMessage,
                Headers = DictionaryHeadersSerialize.Serializer.Serialize(sendContext)
            };

            await p.ProduceAsync(Topic, message);

            var result = await provider.GetTask<ConsumeContext<KafkaMessageWithProducerWrapper>>();

            Assert.AreEqual(message.Value.Test, result.Message.Test);
            Assert.AreEqual(sendContext.MessageId, result.MessageId);
            Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));

            Assert.That(await harness.Consumed.Any<KafkaMessageWithProducerWrapper>());
        }


        class KafkaMessageWithProducerWrapperClass :
            KafkaMessageWithProducerWrapper
        {
            public KafkaMessageWithProducerWrapperClass(string text)
            {
                Test = text;
            }

            public string Test { get; }
        }


        class KafkaMessageWrapperConsumer :
            IConsumer<KafkaMessageWithProducerWrapper>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessageWithProducerWrapper>> _taskCompletionSource;

            public KafkaMessageWrapperConsumer(TaskCompletionSource<ConsumeContext<KafkaMessageWithProducerWrapper>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<KafkaMessageWithProducerWrapper> context)
            {
                _taskCompletionSource.TrySetResult(context);
                return Task.CompletedTask;
            }
        }


        public interface KafkaMessageWithProducerWrapper
        {
            string Test { get; }
        }
    }
