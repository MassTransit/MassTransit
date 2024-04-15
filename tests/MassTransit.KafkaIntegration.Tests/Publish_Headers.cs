namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class Publish_Headers :
        InMemoryTestFixture
    {
        const string OriginalTopic = "original-topic";
        const string FollowingTopic = "following-topic";

        [Test]
        public async Task Should_receive_correct_headers_in_following_message()
        {
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { OriginalTopic, FollowingTopic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<OriginalMessage>>();
                    x.AddTaskCompletionSource<ConsumeContext<FollowingMessage>>();

                    x.AddRider(
                        rider =>
                        {
                            rider.AddConsumer<OriginalMessageConsumer>();
                            rider.AddConsumer<FollowingMessageConsumer>();

                            rider.AddProducer<OriginalMessage>(OriginalTopic);
                            rider.AddProducer<FollowingMessage>(FollowingTopic);

                            rider.UsingKafka(
                                (context, k) =>
                                {
                                    k.TopicEndpoint<OriginalMessage>(
                                        OriginalTopic,
                                        nameof(Receive_Specs),
                                        c =>
                                        {
                                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                            c.ConfigureConsumer<OriginalMessageConsumer>(context);
                                        });

                                    k.TopicEndpoint<FollowingMessage>(
                                        FollowingTopic,
                                        nameof(Receive_Specs),
                                        c =>
                                        {
                                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                            c.ConfigureConsumer<FollowingMessageConsumer>(context);
                                        });
                                });
                        });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            await harness.Start();

            ITopicProducer<OriginalMessage> producer = harness.GetProducer<OriginalMessage>();
            await producer.Produce(new { }, harness.CancellationToken);

            var result = await provider.GetTask<ConsumeContext<OriginalMessage>>();
            var ping = await provider.GetTask<ConsumeContext<FollowingMessage>>();

            Assert.Multiple(() =>
            {
                Assert.That(ping.ConversationId, Is.EqualTo(result.ConversationId));

                Assert.That(ping.SourceAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{OriginalTopic}")));
                Assert.That(ping.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{FollowingTopic}")));

                Assert.That(ping.SourceAddress, Is.EqualTo(result.DestinationAddress));

                Assert.That(ping.MessageId, Is.Not.EqualTo(result.MessageId));
            });
        }


        class OriginalMessageConsumer :
            IConsumer<OriginalMessage>
        {
            readonly ITopicProducer<FollowingMessage> _followingMessageTopicProducer;
            readonly TaskCompletionSource<ConsumeContext<OriginalMessage>> _orginalMessageTaskCompletionSource;

            public OriginalMessageConsumer(TaskCompletionSource<ConsumeContext<OriginalMessage>> orginalMessageTaskCompletionSource,
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
        }


        public interface FollowingMessage
        {
        }
    }
}
