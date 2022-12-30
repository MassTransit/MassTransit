namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class TopicConnector_Specs :
        InMemoryTestFixture
    {
        const string Topic = "endpoint-connector";

        [Test]
        public async Task Should_receive_on_connected_topic()
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
                        rider.AddProducer<KafkaMessage>(Topic);
                        rider.AddConsumer<TestKafkaMessageConsumer<KafkaMessage>>();

                        rider.UsingKafka((_, _) =>
                        {
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();
            await harness.Start();

            var kafka = provider.GetRequiredService<IKafkaRider>();

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            var correlationId = NewId.NextGuid();
            var conversationId = NewId.NextGuid();
            var initiatorId = NewId.NextGuid();
            var messageId = NewId.NextGuid();

            await producer.Produce(new { Text = "text" }, Pipe.Execute<SendContext>(context =>
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
                harness.CancellationToken);

            var connected = kafka.ConnectTopicEndpoint<KafkaMessage>(Topic, nameof(TopicConnector_Specs), (context, configurator) =>
            {
                configurator.AutoOffsetReset = AutoOffsetReset.Earliest;
                configurator.ConfigureConsumer<TestKafkaMessageConsumer<KafkaMessage>>(context);
            });

            await connected.Ready.OrCanceled(harness.CancellationToken);

            var result = await provider.GetTask<ConsumeContext<KafkaMessage>>();

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
