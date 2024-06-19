namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class Long_Receive_Specs :
        InMemoryTestFixture
    {
        const string EventHubName = "receive-eh";

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

                        k.ReceiveEndpoint(EventHubName, Configuration.ConsumerGroup, c =>
                        {
                            c.ConcurrentMessageLimit = 1;
                            c.CheckpointMessageCount = 1;
                            c.CheckpointInterval = TimeSpan.FromSeconds(1);
                            c.ConfigureConsumer<EventHubMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var scope = provider.CreateAsyncScope();

            await busControl.StartAsync(TestCancellationToken);

            var producerProvider = scope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
            var producer = await producerProvider.GetProducer(EventHubName);

            try
            {
                var messageId = NewId.NextGuid();
                await producer.Produce<EventHubMessage>(new { Text = "" }, Pipe.Execute<SendContext>(context => context.MessageId = messageId),
                    TestCancellationToken);

                ConsumeContext<EventHubMessage> result = await taskCompletionSource.Task;

                Assert.That(result.MessageId, Is.EqualTo(messageId));
            }
            finally
            {
                scope.Dispose();

                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
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
                await Task.Delay(TimeSpan.FromSeconds(5));
                _taskCompletionSource.TrySetResult(context);
            }
        }
    }
}
