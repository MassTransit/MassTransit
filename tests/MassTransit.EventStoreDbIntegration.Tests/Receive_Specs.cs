using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EventStore.Client;
using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Serialization;
using MassTransit.TestFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MassTransit.EventStoreDbIntegration.Tests
{
    public class Receive_Specs :
        InMemoryTestFixture
    {
        const string SubscriptionName = "mt_receive_specs_test";
        const string ProducerStreamName = "mt_receive_specs";

        [Test]
        public async Task Should_receive()
        {
            TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> taskCompletionSource = GetTask<ConsumeContext<EventStoreDbMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            _ = services.AddSingleton<EventStoreClient>((provider) =>
            {
                var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
                settings.ConnectionName = "MassTransit Test Connection";

                return new EventStoreClient(settings);
            });

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<EventStoreDbMessageConsumer>();

                    rider.UsingEventStoreDB((context, esdb) =>
                    {
                        esdb.CatchupSubscription(StreamName.Custom(ProducerStreamName), SubscriptionName, c =>
                        {
                            c.CheckpointMessageCount = 1;
                            c.UseEventStoreDBCheckpointStore(StreamName.ForCheckpoint(SubscriptionName));

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

                var message = new EventStoreDbMessageClass("test message");
                var context = new MessageSendContext<EventStoreDbMessage>(message);

                await using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, context);
                    stream.Flush();

                    var metadata = DictionaryHeadersSerde.Serializer.Serialize(context);

                    var preparedMessage = new EventData(
                        Uuid.FromGuid(context.MessageId.Value),
                        message.GetType().Name,
                        stream.ToArray(),
                        metadata);

                    await producer.AppendToStreamAsync(StreamName.Custom(ProducerStreamName), StreamState.Any, new List<EventData> { preparedMessage });
                }

                ConsumeContext<EventStoreDbMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual(message.Text, result.Message.Text);
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);
                await provider.DisposeAsync();
            }
        }


        public class EventStoreDbMessageClass :
            EventStoreDbMessage
        {
            public EventStoreDbMessageClass(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }


        public class EventStoreDbMessageConsumer :
            IConsumer<EventStoreDbMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> _taskCompletionSource;

            public EventStoreDbMessageConsumer(TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<EventStoreDbMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface EventStoreDbMessage
        {
            string Text { get; }
        }
    }
}
