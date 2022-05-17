namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class Long_Receive_Specs :
        InMemoryTestFixture
    {
        const string Topic = "long-receive-test";

        [Test]
        public async Task Should_receive()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
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

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(Long_Receive_Specs), c =>
                        {
                            c.CreateIfMissing();
                            c.ConcurrentMessageLimit = 1;
                            c.CheckpointMessageCount = 1;
                            c.CheckpointInterval = TimeSpan.FromSeconds(1);
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var scope = provider.CreateScope();

            await busControl.StartAsync(TestCancellationToken);

            var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            try
            {
                var messageId = NewId.NextGuid();
                await producer.Produce(new { }, Pipe.Execute<SendContext>(context => context.MessageId = messageId), TestCancellationToken);

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual(messageId, result.MessageId);
            }
            finally
            {
                scope.Dispose();

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
                await Task.Delay(TimeSpan.FromSeconds(5));
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface KafkaMessage
        {
        }
    }
}
