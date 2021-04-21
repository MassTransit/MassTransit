namespace MassTransit.EventStoreDbIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EventStore.Client;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using Util;


    public class BatchProducer_Specs :
        InMemoryTestFixture
    {
        const int Expected = 10;
        const string SubscriptionName = "mt_batch_producer_specs_test";
        const string ProducerStreamName = "mt_batch_producer_specs";

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

            var consumer = new EventStoreDbMessageConsumer(taskCompletionSource, Expected);
            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.UsingEventStoreDB((context, esdb) =>
                    {
                        esdb.CatchupSubscription(StreamName.Custom(ProducerStreamName), SubscriptionName, c =>
                        {
                            c.UseEventStoreDBCheckpointStore(StreamName.ForCheckpoint(SubscriptionName));
                            c.Consumer(() => consumer);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventStoreDbProducerProvider>();
            var producer = await producerProvider.GetProducer(StreamName.Custom(ProducerStreamName));

            try
            {
                List<EventStoreDbMessageClass> messages = Enumerable.Range(0, Expected)
                    .Select(x => new EventStoreDbMessageClass(x.ToString()))
                    .ToList();

                var correlationId = NewId.NextGuid();
                var conversationId = NewId.NextGuid();
                var initiatorId = NewId.NextGuid();
                var messageId = NewId.NextGuid();
                await producer.Produce<EventStoreDbMessage>(messages, Pipe.Execute<SendContext>(context =>
                {
                    context.CorrelationId = correlationId;
                    context.MessageId = messageId;
                    context.InitiatorId = initiatorId;
                    context.ConversationId = conversationId;
                }), TestCancellationToken);

                ConsumeContext<EventStoreDbMessage> result = await taskCompletionSource.Task;

                Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                Assert.That(result.DestinationAddress,
                    Is.EqualTo(new Uri($"loopback://localhost/{EventStoreDbEndpointAddress.PathPrefix}/{StreamName.Custom(ProducerStreamName)}")));
                Assert.That(result.MessageId, Is.EqualTo(messageId));
                Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
                Assert.That(result.InitiatorId, Is.EqualTo(initiatorId));
                Assert.That(result.ConversationId, Is.EqualTo(conversationId));
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
            readonly int _expected;
            readonly TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> _taskCompletionSource;
            int _consumed;

            public EventStoreDbMessageConsumer(TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> taskCompletionSource, int expected)
            {
                _consumed = 0;
                _taskCompletionSource = taskCompletionSource;
                _expected = expected;
            }

            public Task Consume(ConsumeContext<EventStoreDbMessage> context)
            {
                if (Interlocked.Increment(ref _consumed) == _expected)
                {
                    _taskCompletionSource.TrySetResult(context);
                    Interlocked.Exchange(ref _consumed, 0);
                }

                return TaskUtil.Completed;
            }
        }


        public interface EventStoreDbMessage
        {
            string Text { get; }
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
    }
}
