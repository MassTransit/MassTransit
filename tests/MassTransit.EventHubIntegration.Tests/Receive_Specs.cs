namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serialization;
    using TestFramework;


    public class Receive_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive()
        {
            TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource = GetTask<ConsumeContext<EventHubMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

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

                        k.Subscribe(Configuration.EventHubName, c =>
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
                await using var stream = new MemoryStream();
                await using var writer = new StreamWriter(stream);
                JsonMessageSerializer.Serializer.Serialize(writer, message);

                await writer.FlushAsync();

                var eventData = new EventData(stream.ToArray());
                await producer.SendAsync(new[] {eventData});
                ConsumeContext<EventHubMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual(message.Text, result.Message.Text);
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/event-hub/{Configuration.EventHubName}")));
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
            readonly TaskCompletionSource<ConsumeContext<EventHubMessage>> _taskCompletionSource;

            public EventHubMessageConsumer(TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<EventHubMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface EventHubMessage
        {
            string Text { get; }
        }
    }
}
