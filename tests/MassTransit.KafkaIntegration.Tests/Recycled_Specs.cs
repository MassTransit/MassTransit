namespace MassTransit.KafkaIntegration.Tests
{
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class Recycled_Specs :
        InMemoryTestFixture
    {
        const string Topic = "recycle";

        [Test]
        public async Task Should_receive_after_recycle()
        {
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<TestKafkaMessageConsumer<KafkaMessage>>();

                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Recycled_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConfigureConsumer<TestKafkaMessageConsumer<KafkaMessage>>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            var busControl = provider.GetRequiredService<IBusControl>();

            //Start all hosted services
            await harness.Start();

            await busControl.StopAsync(harness.CancellationToken);

            await Task.Delay(500, harness.CancellationToken);

            await busControl.StartAsync(harness.CancellationToken);

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            await producer.Produce(new { }, harness.CancellationToken);
            await provider.GetTask<ConsumeContext<KafkaMessage>>();
        }


        public interface KafkaMessage
        {
        }
    }
}
