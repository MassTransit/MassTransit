namespace MassTransit.EventStoreDbIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using EventStore.Client;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class Producer_Specs :
        InMemoryTestFixture
    {
        const string SubscriptionName = "mt_producer_specs_test";
        const string ProducerStreamName = "mt_producer_specs";

        [Test]
        public async Task Should_produce()
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
                        esdb.CatchupSubscription(StreamCategory.FromString(ProducerStreamName), SubscriptionName, c =>
                        {
                            c.UseEventStoreDBCheckpointStore(StreamName.ForCheckpoint(SubscriptionName));

                            c.ConfigureConsumer<EventStoreDbMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventStoreDbProducerProvider>();
            var producer = await producerProvider.GetProducer(ProducerStreamName);

            try
            {
                var correlationId = NewId.NextGuid();
                var conversationId = NewId.NextGuid();
                var initiatorId = NewId.NextGuid();
                var messageId = NewId.NextGuid();
                await producer.Produce<EventStoreDbMessage>(new {Text = "text"}, Pipe.Execute<SendContext>(context =>
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

                ConsumeContext<EventStoreDbMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual("text", result.Message.Text);
                Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                Assert.That(result.DestinationAddress,
                    Is.EqualTo(new Uri($"loopback://localhost/{EventStoreDbEndpointAddress.PathPrefix}/{ProducerStreamName}")));
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

                await provider.DisposeAsync();
            }
        }


        class EventStoreDbMessageConsumer :
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


        public interface HeaderType
        {
            string Key { get; }
            string Value { get; }
        }


        public interface EventStoreDbMessage
        {
            string Text { get; }
        }
    }
}
