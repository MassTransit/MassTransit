namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class TopicConnector_Specs :
        InMemoryTestFixture
    {
        const string Topic = "endpoint-connector";

        [Test]
        public async Task Should_produce()
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
                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var kafka = provider.GetRequiredService<IKafkaRider>();

            var serviceScope = provider.CreateScope();

            var producer = serviceScope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            try
            {
                var correlationId = NewId.NextGuid();
                var conversationId = NewId.NextGuid();
                var initiatorId = NewId.NextGuid();
                var messageId = NewId.NextGuid();

                await producer.Produce(new {Text = "text"}, Pipe.Execute<SendContext>(context =>
                    {
                        context.CorrelationId = correlationId;
                        context.MessageId = messageId;
                        context.InitiatorId = initiatorId;
                        context.ConversationId = conversationId;
                        context.Headers.Set("Special", new
                        {
                            Key = "Hello",
                            Value = "World"
                        });
                    }),
                    TestCancellationToken);

                var connected = kafka.ConnectTopicEndpoint<KafkaMessage>(Topic, nameof(TopicConnector_Specs), (context, configurator) =>
                {
                    configurator.AutoOffsetReset = AutoOffsetReset.Earliest;
                    configurator.ConfigureConsumer<KafkaMessageConsumer>(context);
                });

                await connected.Ready;

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual("text", result.Message.Text);
                Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));
                Assert.That(result.MessageId, Is.EqualTo(messageId));
                Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
                Assert.That(result.InitiatorId, Is.EqualTo(initiatorId));
                Assert.That(result.ConversationId, Is.EqualTo(conversationId));

                var headerType = result.Headers.Get<HeaderType>("Special");
                Assert.That(headerType, Is.Not.Null);
                Assert.That(headerType.Key, Is.EqualTo("Hello"));
                Assert.That(headerType.Value, Is.EqualTo("World"));
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


        public interface HeaderType
        {
            string Key { get; }
            string Value { get; }
        }


        public interface KafkaMessage
        {
            string Text { get; }
        }
    }
}
