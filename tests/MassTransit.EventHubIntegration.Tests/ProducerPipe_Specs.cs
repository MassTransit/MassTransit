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


    public class ProducerPipe_Specs :
        InMemoryTestFixture
    {
        const string EventHubName = "produce-eh";

        [Test]
        public async Task Should_produce()
        {
            TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource = GetTask<ConsumeContext<EventHubMessage>>();
            TaskCompletionSource<SendContext> sendFilterTaskCompletionSource = GetTask<SendContext>();
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
                        k.ConfigureSend(s => s.UseFilter(new SendFilter(sendFilterTaskCompletionSource)));
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
                await producer.Produce<EventHubMessage>(new { Text = "text" }, TestCancellationToken);

                var result = await sendFilterTaskCompletionSource.Task;

                Assert.Multiple(() =>
                {
                    Assert.That(result.TryGetPayload<EventHubSendContext>(out _), Is.True);
                    Assert.That(result.TryGetPayload<EventHubSendContext<EventHubMessage>>(out _), Is.True);
                    Assert.That(result.DestinationAddress,
                        Is.EqualTo(new Uri($"loopback://localhost/{EventHubEndpointAddress.PathPrefix}/{EventHubName}")));
                });

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
                _taskCompletionSource.TrySetResult(context);
            }
        }


        class SendFilter :
            IFilter<SendContext>
        {
            readonly TaskCompletionSource<SendContext> _taskCompletionSource;

            public SendFilter(TaskCompletionSource<SendContext> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Send(SendContext context, IPipe<SendContext> next)
            {
                _taskCompletionSource.TrySetResult(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
