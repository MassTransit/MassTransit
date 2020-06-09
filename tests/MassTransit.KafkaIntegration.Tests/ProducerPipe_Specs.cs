namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using Transport;


    public class ProducerPipe_Specs :
        InMemoryTestFixture
    {
        const string Topic = "producer-pipe";

        [Test]
        public async Task Should_produce()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
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
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<Null, KafkaMessage>(Topic, nameof(ProducerPipe_Specs), c =>
                        {
                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });

                        k.TopicProducer<Null, KafkaMessage>(Topic, c =>
                        {
                            c.ConfigureSend(s => s.UseFilter(new SendFilter(sendFilterTaskCompletionSource)));
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producer = serviceScope.ServiceProvider.GetRequiredService<IKafkaProducer<Null, KafkaMessage>>();

            try
            {
                var correlationId = NewId.NextGuid();
                await producer.Produce(new
                {
                    CorrelationId = correlationId,
                    Text = "text"
                }, TestCancellationToken);

                var result = await sendFilterTaskCompletionSource.Task;

                Assert.AreEqual(correlationId, result.CorrelationId);
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/kafka/{Topic}")));

                await taskCompletionSource.Task;
            }
            finally
            {
                serviceScope.Dispose();

                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
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


        public interface KafkaMessage
        {
            Guid CorrelationId { get; }
            string Text { get; }
        }
    }
}
