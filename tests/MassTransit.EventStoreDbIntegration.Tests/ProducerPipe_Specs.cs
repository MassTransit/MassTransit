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


    public class ProducerPipe_Specs :
        InMemoryTestFixture
    {
        const string SubscriptionName = "mt_producer_pipe_specs_test";
        const string ProducerStreamName = "mt_producer_pipe_specs";

        [Test]
        public async Task Should_produce()
        {
            TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> taskCompletionSource = GetTask<ConsumeContext<EventStoreDbMessage>>();
            TaskCompletionSource<SendContext> sendFilterTaskCompletionSource = GetTask<SendContext>();
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
                        esdb.CatchupSubscription(StreamName.Custom(ProducerStreamName), SubscriptionName, c =>
                        {
                            c.UseEventStoreDBCheckpointStore(StreamName.ForCheckpoint(SubscriptionName));
                            c.ConfigureConsumer<EventStoreDbMessageConsumer>(context);
                        });
                        esdb.ConfigureSend(s => s.UseFilter(new SendFilter(sendFilterTaskCompletionSource)));
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
                var correlationId = NewId.NextGuid();
                await producer.Produce<EventStoreDbMessage>(new
                {
                    CorrelationId = correlationId,
                    Text = "text"
                }, TestCancellationToken);

                var result = await sendFilterTaskCompletionSource.Task;

                Assert.IsTrue(result.TryGetPayload<EventStoreDbSendContext>(out _));
                Assert.IsTrue(result.TryGetPayload<EventStoreDbSendContext<EventStoreDbMessage>>(out _));
                Assert.AreEqual(correlationId, result.CorrelationId);
                Assert.That(result.DestinationAddress,
                    Is.EqualTo(new Uri($"loopback://localhost/{EventStoreDbEndpointAddress.PathPrefix}/{StreamName.Custom(ProducerStreamName)}")));

                await taskCompletionSource.Task;
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


        public interface EventStoreDbMessage
        {
            Guid CorrelationId { get; }
            string Text { get; }
        }
    }
}
