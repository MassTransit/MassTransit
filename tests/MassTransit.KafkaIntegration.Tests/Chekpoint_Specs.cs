namespace MassTransit.KafkaIntegration.Tests
{
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using Util;


    public class Chekpoint_Specs :
        InMemoryTestFixture
    {
        const string Topic = "test-checkpoint";

        [Test]
        public async Task Should_call_checkpoint_pipe()
        {
            TaskCompletionSource<CheckpointContext> taskCompletionSource = GetTask<CheckpointContext>();
            var services = new ServiceCollection();

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

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(Chekpoint_Specs), c =>
                        {
                            c.CheckpointMessageCount = 1;
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);

                            c.UseFilter(new CheckpointFilter(taskCompletionSource));
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var observer = GetConsumeObserver();
            busControl.ConnectConsumeObserver(observer);

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                const int partition = 0;
                IPipe<KafkaSendContext<KafkaMessage>> pipe = Pipe.Execute<KafkaSendContext<KafkaMessage>>(x => x.Partition = partition);
                var producer = provider.GetRequiredService<ITopicProducer<KafkaMessage>>();

                await producer.Produce(new { }, pipe, TestCancellationToken);

                var context = await taskCompletionSource.Task;

                Assert.AreEqual(Topic, context.Topic);
                Assert.AreEqual(partition, context.Partition);

                Assert.That(await observer.Messages.Any<KafkaMessage>());
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        class CheckpointFilter :
            IFilter<CheckpointContext>
        {
            readonly TaskCompletionSource<CheckpointContext> _taskCompletionSource;

            public CheckpointFilter(TaskCompletionSource<CheckpointContext> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(CheckpointContext context, IPipe<CheckpointContext> next)
            {
                _taskCompletionSource.TrySetResult(context);
                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            public Task Consume(ConsumeContext<KafkaMessage> context)
            {
                return TaskUtil.Completed;
            }
        }


        public interface KafkaMessage
        {
        }
    }
}
