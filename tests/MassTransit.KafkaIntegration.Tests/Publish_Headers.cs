namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serializers;
    using TestFramework;


    public class Publish_Headers :
        InMemoryTestFixture
    {
        const string OriginalTopic = "original-topic";
        const string FollowingTopic = "following-topic";

        [Test]
        public async Task Should_receive_correct_headers_in_following_message()
        {
            TaskCompletionSource<ConsumeContext<OriginalMessage>> taskCompletionSource = GetTask<ConsumeContext<OriginalMessage>>();
            TaskCompletionSource<ConsumeContext<FollowingMessage>> pingTaskCompletionSource = GetTask<ConsumeContext<FollowingMessage>>();

            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);
            services.AddSingleton(pingTaskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(
                x =>
                {
                    x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                    x.AddRider(
                        rider =>
                        {
                            rider.AddConsumer<OriginalMessageConsumer>();
                            rider.AddConsumer<FollowingMessageConsumer>();

                            rider.AddProducer<FollowingMessage>(FollowingTopic);

                            rider.UsingKafka(
                                (context, k) =>
                                {
                                    k.Host("localhost:9092");

                                    k.TopicEndpoint<OriginalMessage>(
                                        OriginalTopic,
                                        nameof(Receive_Specs),
                                        c =>
                                        {
                                            c.CreateIfMissing();
                                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                            c.ConfigureConsumer<OriginalMessageConsumer>(context);
                                        });

                                    k.TopicEndpoint<FollowingMessage>(
                                        FollowingTopic,
                                        nameof(Receive_Specs),
                                        c =>
                                        {
                                            c.CreateIfMissing();
                                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                            c.ConfigureConsumer<FollowingMessageConsumer>(context);
                                        });
                                });
                        });
                });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

                using IProducer<Null, OriginalMessage> p = new ProducerBuilder<Null, OriginalMessage>(config)
                    .SetValueSerializer(new MassTransitJsonSerializer<OriginalMessage>())
                    .Build();

                var originalMessage = new OriginalMessageClass("test");
                var sendContext = new MessageSendContext<OriginalMessage>(originalMessage)
                {
                    ConversationId = NewId.NextGuid(),
                };
                var message = new Message<Null, OriginalMessage>
                {
                    Value = originalMessage,
                    Headers = DictionaryHeadersSerialize.Serializer.Serialize(sendContext)
                };

                await p.ProduceAsync(OriginalTopic, message);

                ConsumeContext<OriginalMessage> result = await taskCompletionSource.Task;
                ConsumeContext<FollowingMessage> ping = await pingTaskCompletionSource.Task;

                Assert.AreEqual(result.ConversationId, ping.ConversationId);

                Assert.That(ping.SourceAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{OriginalTopic}")));
                Assert.That(ping.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{FollowingTopic}")));
                Assert.AreEqual(result.DestinationAddress, ping.SourceAddress);

                Assert.AreNotEqual(result.MessageId, ping.MessageId);
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        class OriginalMessageClass :
            OriginalMessage
        {
            public OriginalMessageClass(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }


        class OriginalMessageConsumer :
            IConsumer<OriginalMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<OriginalMessage>> _orginalMessageTaskCompletionSource;
            readonly ITopicProducer<FollowingMessage> _followingMessageTopicProducer;

            public OriginalMessageConsumer(
                TaskCompletionSource<ConsumeContext<OriginalMessage>> orginalMessageTaskCompletionSource,
                ITopicProducer<FollowingMessage> followingMessageTopicProducer)
            {
                _orginalMessageTaskCompletionSource = orginalMessageTaskCompletionSource;
                _followingMessageTopicProducer = followingMessageTopicProducer;
            }

            public async Task Consume(ConsumeContext<OriginalMessage> context)
            {
                _orginalMessageTaskCompletionSource.TrySetResult(context);

                await _followingMessageTopicProducer.Produce(new { });
            }
        }


        class FollowingMessageConsumer :
            IConsumer<FollowingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<FollowingMessage>> _taskCompletionSource;

            public FollowingMessageConsumer(TaskCompletionSource<ConsumeContext<FollowingMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<FollowingMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
                return Task.CompletedTask;
            }
        }


        public interface OriginalMessage
        {
            string Text { get; }
        }


        public interface FollowingMessage
        {
        }
    }
}
