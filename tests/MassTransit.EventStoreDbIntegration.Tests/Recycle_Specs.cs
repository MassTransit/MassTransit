namespace MassTransit.EventStoreDbIntegration.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EventStore.Client;
    using GreenPipes;
    using MassTransit.EventStoreDbIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class Recycle_Specs :
        InMemoryTestFixture
    {
        const string SubscriptionName = "mt_recycle_specs_test";
        const string ProducerStreamName = "mt_recycle_specs";

        public Recycle_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }

        [Test]
        public async Task Should_produce()
        {
            const int numOfTries = 2;

            TaskCompletionSource<ConsumeContext<EventStoreDbMessage>>[] taskCompletionSources = Enumerable.Range(0, numOfTries)
                .Select(x => GetTask<ConsumeContext<EventStoreDbMessage>>())
                .ToArray();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSources);

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
                        //esdb.Host("esdb://localhost:2113?tls=false", "MassTransit Recycle_Specs Connection");

                        esdb.CatchupSubscription(StreamName.Custom(ProducerStreamName), SubscriptionName, c =>
                        {
                            c.CheckpointMessageCount = 1;
                            c.UseEventStoreDBCheckpointStore(StreamName.ForCheckpoint(SubscriptionName));

                            c.ConfigureConsumer<EventStoreDbMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            try
            {
                for (var i = 0; i < numOfTries; i++)
                {
                    await busControl.StartAsync(TestCancellationToken);

                    var serviceScope = provider.CreateScope();

                    var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventStoreDbProducerProvider>();
                    var producer = await producerProvider.GetProducer(StreamName.Custom(ProducerStreamName));

                    try
                    {
                        var correlationId = NewId.NextGuid();
                        var conversationId = NewId.NextGuid();
                        var initiatorId = NewId.NextGuid();
                        var messageId = NewId.NextGuid();
                        await producer.Produce<EventStoreDbMessage>(new
                            {
                                Text = "text",
                                Index = i
                            },
                            Pipe.Execute<SendContext>(context =>
                            {
                                context.CorrelationId = correlationId;
                                context.MessageId = messageId;
                                context.InitiatorId = initiatorId;
                                context.ConversationId = conversationId;
                                context.Headers.Set("Special", new
                                {
                                    Key = "Hello",
                                    Value = "World"
                                });
                            }),
                            TestCancellationToken);

                        TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> taskCompletionSource = taskCompletionSources[i];
                        ConsumeContext<EventStoreDbMessage> result = await taskCompletionSource.Task;

                        Assert.AreEqual("text", result.Message.Text);
                        Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                        Assert.That(result.DestinationAddress,
                            Is.EqualTo(new Uri($"loopback://localhost/{EventStoreDbEndpointAddress.PathPrefix}/{StreamName.Custom(ProducerStreamName)}")));
                        Assert.That(result.MessageId, Is.EqualTo(messageId));
                        Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
                        Assert.That(result.InitiatorId, Is.EqualTo(initiatorId));
                        Assert.That(result.ConversationId, Is.EqualTo(conversationId));

                        var headerType = result.Headers.Get<HeaderType>("Special");
                        Assert.That(headerType, Is.Not.Null);
                        Assert.That(headerType.Key, Is.EqualTo("Hello"));
                        Assert.That(headerType.Value, Is.EqualTo("World"));
                    }
                    finally
                    {
                        serviceScope.Dispose();

                        await busControl.StopAsync(TestCancellationToken);

                        await Task.Delay(100, TestCancellationToken);
                    }
                }
            }
            finally
            {
                await provider.DisposeAsync();
            }
        }


        class EventStoreDbMessageConsumer :
            IConsumer<EventStoreDbMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<EventStoreDbMessage>>[] _taskCompletionSource;

            public EventStoreDbMessageConsumer(TaskCompletionSource<ConsumeContext<EventStoreDbMessage>>[] taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<EventStoreDbMessage> context)
            {
                _taskCompletionSource[context.Message.Index].TrySetResult(context);
            }
        }


        public interface HeaderType
        {
            string Key { get; }
            string Value { get; }
        }


        public interface EventStoreDbMessage
        {
            int Index { get; }
            string Text { get; }
        }
    }
}
