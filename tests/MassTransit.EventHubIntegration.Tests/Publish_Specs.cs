namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;
    using Context;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serialization;
    using TestFramework;


    public class Publish_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive()
        {
            TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource = GetTask<ConsumeContext<EventHubMessage>>();
            TaskCompletionSource<ConsumeContext<BusPing>> pingTaskCompletionSource = GetTask<ConsumeContext<BusPing>>();

            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);
            services.AddSingleton(pingTaskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<BusPingConsumer>();
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

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                await using var producer = new EventHubProducerClient(Configuration.EventHubNamespace, Configuration.EventHubName);

                var message = new EventHubMessageClass("test");
                var context = new MessageSendContext<EventHubMessage>(message)
                {
                    Serializer = SystemTextJsonMessageSerializer.Instance,
                    CorrelationId = NewId.NextGuid()
                };

                var eventData = new EventData(SystemTextJsonMessageSerializer.Instance.GetMessageBody(context).GetBytes());
                await producer.SendAsync(new[] { eventData });

                ConsumeContext<EventHubMessage> result = await taskCompletionSource.Task;
                ConsumeContext<BusPing> ping = await pingTaskCompletionSource.Task;

                Assert.AreEqual(message.Text, result.Message.Text);

                Assert.AreEqual(result.CorrelationId, ping.InitiatorId);
                Assert.That(ping.SourceAddress, Is.EqualTo(new Uri($"loopback://localhost/{EventHubEndpointAddress.PathPrefix}/{Configuration.EventHubName}")));
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
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


        class EventHubMessageConsumer :
            IConsumer<EventHubMessage>
        {
            readonly IPublishEndpoint _publishEndpoint;
            readonly TaskCompletionSource<ConsumeContext<EventHubMessage>> _taskCompletionSource;

            public EventHubMessageConsumer(IPublishEndpoint publishEndpoint, TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource)
            {
                _publishEndpoint = publishEndpoint;
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<EventHubMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
                await _publishEndpoint.Publish<BusPing>(new { });
            }
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
                return Task.CompletedTask;
            }
        }


        public interface BusPing
        {
        }
    }
}
