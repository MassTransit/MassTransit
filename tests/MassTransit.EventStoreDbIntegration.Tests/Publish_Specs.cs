namespace MassTransit.EventStoreDbIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Context;
    using EventStore.Client;
    using GreenPipes.Util;
    using MassTransit.EventStoreDbIntegration.Serializers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serialization;
    using TestFramework;


    public class Publish_Specs :
        InMemoryTestFixture
    {
        const string SubscriptionName = "mt_publish_specs";
        const string CheckpointId = "mt_publish_specs";

        [Test]
        public async Task Should_receive()
        {
            TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> taskCompletionSource = GetTask<ConsumeContext<EventStoreDbMessage>>();
            TaskCompletionSource<ConsumeContext<BusPing>> pingTaskCompletionSource = GetTask<ConsumeContext<BusPing>>();

            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);
            services.AddSingleton(pingTaskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            _ = services.AddSingleton<EventStoreClient>((provider) =>
            {
                var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
                settings.ConnectionName = "MassTransit Publish_Specs Connection";

                return new EventStoreClient(settings);
            });

            services.AddMassTransit(x =>
            {
                x.AddConsumer<BusPingConsumer>();
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<EventStoreDbMessageConsumer>();

                    rider.UsingEventStoreDB((context, esdb) =>
                    {
                        esdb.UseExistingClient();

                        esdb.CatchupSubscription(StreamCategory.AllStream, SubscriptionName, c =>
                        {
                            c.UseEventStoreDBCheckpointStore(StreamName.ForCheckpoint(CheckpointId));
                            c.ConfigureConsumer<EventStoreDbMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                using var producer = provider.GetRequiredService<EventStoreClient>();

                var serializer = new JsonMessageSerializer();

                var message = new EventStoreDbMessageClass("test");
                var context = new MessageSendContext<EventStoreDbMessage>(message)
                {
                    Serializer = serializer,
                    CorrelationId = NewId.NextGuid()
                };

                await using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, context);
                    stream.Flush();

                    var metadata = DictionaryHeadersSerialize.Serializer.Serialize(context);

                    var preparedMessage = new EventData(
                        Uuid.FromGuid(context.MessageId.Value),
                        message.GetType().Name,
                        stream.ToArray(),
                        metadata);

                    await producer.AppendToStreamAsync(StreamName.Custom("mt_publish_specs"), StreamState.Any, new List<EventData> { preparedMessage });
                }

                ConsumeContext<EventStoreDbMessage> result = await taskCompletionSource.Task;
                ConsumeContext<BusPing> ping = await pingTaskCompletionSource.Task;

                Assert.AreEqual(message.Text, result.Message.Text);

                Assert.AreEqual(result.CorrelationId, ping.InitiatorId);
                Assert.That(ping.SourceAddress, Is.EqualTo(new Uri($"loopback://localhost/{EventStoreDbEndpointAddress.PathPrefix}/{StreamCategory.AllStream}/{SubscriptionName}")));
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        class EventStoreDbMessageClass :
            EventStoreDbMessage
        {
            public EventStoreDbMessageClass(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }


        class EventStoreDbMessageConsumer :
            IConsumer<EventStoreDbMessage>
        {
            readonly IPublishEndpoint _publishEndpoint;
            readonly TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> _taskCompletionSource;

            public EventStoreDbMessageConsumer(IPublishEndpoint publishEndpoint, TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> taskCompletionSource)
            {
                _publishEndpoint = publishEndpoint;
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<EventStoreDbMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
                await _publishEndpoint.Publish<BusPing>(new { });
            }
        }


        public interface EventStoreDbMessage
        {
            string Text { get; }
        }


        class BusPingConsumer :
            IConsumer<BusPing>
        {
            readonly TaskCompletionSource<ConsumeContext<BusPing>> _taskCompletionSource;

            public BusPingConsumer(TaskCompletionSource<ConsumeContext<BusPing>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<BusPing> context)
            {
                _taskCompletionSource.TrySetResult(context);
                return TaskUtil.Completed;
            }
        }


        public interface BusPing
        {}
    }
}
