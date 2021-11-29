namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class BatchProducer_Specs :
        InMemoryTestFixture
    {
        const int Expected = 10;

        [Test]
        public async Task Should_produce()
        {
            TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource = GetTask<ConsumeContext<EventHubMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            var consumer = new EventHubMessageConsumer(taskCompletionSource, Expected);
            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.UsingEventHub((context, k) =>
                    {
                        k.Host(Configuration.EventHubNamespace);
                        k.Storage(Configuration.StorageAccount);

                        k.ReceiveEndpoint(Configuration.EventHubName, c =>
                        {
                            c.Consumer(() => consumer);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
            var producer = await producerProvider.GetProducer(Configuration.EventHubName);

            try
            {
                List<EventHubMessageClass> messages = Enumerable.Range(0, Expected)
                    .Select(x => new EventHubMessageClass(x.ToString()))
                    .ToList();

                var correlationId = NewId.NextGuid();
                var conversationId = NewId.NextGuid();
                var initiatorId = NewId.NextGuid();
                var messageId = NewId.NextGuid();
                await producer.Produce<EventHubMessage>(messages, Pipe.Execute<SendContext>(context =>
                {
                    context.CorrelationId = correlationId;
                    context.MessageId = messageId;
                    context.InitiatorId = initiatorId;
                    context.ConversationId = conversationId;
                }), TestCancellationToken);

                ConsumeContext<EventHubMessage> result = await taskCompletionSource.Task;

                Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                Assert.That(result.DestinationAddress,
                    Is.EqualTo(new Uri($"loopback://localhost/{EventHubEndpointAddress.PathPrefix}/{Configuration.EventHubName}")));
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


        class EventHubMessageConsumer :
            IConsumer<EventHubMessage>
        {
            readonly int _expected;
            readonly TaskCompletionSource<ConsumeContext<EventHubMessage>> _taskCompletionSource;
            int _consumed;

            public EventHubMessageConsumer(TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource, int expected)
            {
                _consumed = 0;
                _taskCompletionSource = taskCompletionSource;
                _expected = expected;
            }

            public Task Consume(ConsumeContext<EventHubMessage> context)
            {
                if (Interlocked.Increment(ref _consumed) == _expected)
                {
                    _taskCompletionSource.TrySetResult(context);
                    Interlocked.Exchange(ref _consumed, 0);
                }

                return Task.CompletedTask;
            }
        }


        class EventHubMessageClass :
            EventHubMessage
        {
            public EventHubMessageClass(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }
    }
}
