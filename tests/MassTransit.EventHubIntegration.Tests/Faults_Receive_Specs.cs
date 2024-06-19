namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class Faults_Receive_Specs :
        InMemoryTestFixture
    {
        const string EventHubName = "default-eh";

        [Test]
        public async Task Should_produce()
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
                            c.ConfigureConsumer<EventHubMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateAsyncScope();

            var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
            var producer = await producerProvider.GetProducer(EventHubName);

            try
            {
                await producer.Produce<EventHubMessage>(new { Index = 0 }, TestCancellationToken);
                await producer.Produce<EventHubMessage>(new { Index = 1 }, TestCancellationToken);

                await taskCompletionSource.Task;
            }
            finally
            {
                await serviceScope.DisposeAsync();

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
                if (context.Message.Index == 0)
                    throw new ArgumentException("Expected failure");

                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface EventHubMessage
        {
            int Index { get; }
        }
    }
}
