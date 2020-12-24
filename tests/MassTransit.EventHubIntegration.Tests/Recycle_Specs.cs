namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class Recycled_Specs :
        InMemoryTestFixture
    {
        public Recycled_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }

        [Test]
        public async Task Should_produce()
        {
            const int numOfTries = 2;

            TaskCompletionSource<ConsumeContext<EventHubMessage>>[] taskCompletionSources = Enumerable.Range(0, numOfTries)
                .Select(x => GetTask<ConsumeContext<EventHubMessage>>())
                .ToArray();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSources);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<EventHubMessageConsumer>();

                    rider.UsingEventHub((context, k) =>
                    {
                        k.Host(Configuration.EventHubNamespace);
                        k.Storage(Configuration.StorageAccount);

                        k.ReceiveEndpoint(Configuration.EventHubName, c =>
                        {
                            c.ConfigureConsumer<EventHubMessageConsumer>(context);
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

                    var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
                    var producer = await producerProvider.GetProducer(Configuration.EventHubName);

                    try
                    {
                        var correlationId = NewId.NextGuid();
                        var conversationId = NewId.NextGuid();
                        var initiatorId = NewId.NextGuid();
                        var messageId = NewId.NextGuid();
                        await producer.Produce<EventHubMessage>(new
                            {
                                Text = "text",
                                Index = i
                            }, Pipe.Execute<SendContext>(context =>
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

                        TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource = taskCompletionSources[i];
                        ConsumeContext<EventHubMessage> result = await taskCompletionSource.Task;

                        Assert.AreEqual("text", result.Message.Text);
                        Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                        Assert.That(result.DestinationAddress,
                            Is.EqualTo(new Uri($"loopback://localhost/{EventHubEndpointAddress.PathPrefix}/{Configuration.EventHubName}")));
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


        class EventHubMessageConsumer :
            IConsumer<EventHubMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<EventHubMessage>>[] _taskCompletionSource;

            public EventHubMessageConsumer(TaskCompletionSource<ConsumeContext<EventHubMessage>>[] taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<EventHubMessage> context)
            {
                _taskCompletionSource[context.Message.Index].TrySetResult(context);
            }
        }


        public interface HeaderType
        {
            string Key { get; }
            string Value { get; }
        }


        public interface EventHubMessage
        {
            int Index { get; }
            string Text { get; }
        }
    }
}
