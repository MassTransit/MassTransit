namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class Publishing_a_message_to_the_bus_through_the_outbox :
        InMemoryTestFixture
    {
        const string EventHubName = "publish-eh";

        [Test]
        public async Task Should_use_the_default_endpoint_serializer()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (BusMessage _) =>
                    {
                    });

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(30));

                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<MessageHandler>();
                        rider.AddInMemoryInboxOutbox();

                        rider.UsingEventHub((context, k) =>
                        {
                            k.Host(Configuration.EventHubNamespace);
                            k.Storage(Configuration.StorageAccount);

                            k.ReceiveEndpoint(EventHubName, Configuration.ConsumerGroup, c =>
                            {
                                c.UseInMemoryInboxOutbox(context);

                                c.ConfigureConsumer<MessageHandler>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();

            await harness.Start();

            var producer = await harness.GetProducer(EventHubName);

            var correlationId = NewId.NextGuid();
            var conversationId = NewId.NextGuid();
            var initiatorId = NewId.NextGuid();
            var messageId = NewId.NextGuid();
            await producer.Produce<EventHubMessage>(new { Test = "text" }, Pipe.Execute<SendContext>(context =>
                {
                    context.CorrelationId = correlationId;
                    context.MessageId = messageId;
                    context.InitiatorId = initiatorId;
                    context.ConversationId = conversationId;
                }),
                harness.CancellationToken);

            IReceivedMessage<BusMessage> message = await harness.Consumed.SelectAsync<BusMessage>().FirstOrDefault();

            Assert.That(message, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(message.Context.Message.OriginalMessageId, Is.EqualTo(messageId));
                Assert.That(message.Context.Message.OriginalCorrelationId, Is.EqualTo(correlationId));
            });
        }


        class MessageHandler :
            IConsumer<EventHubMessage>
        {
            public Task Consume(ConsumeContext<EventHubMessage> context)
            {
                return context.Publish(new BusMessage(context.MessageId, context.CorrelationId));
            }
        }


        record BusMessage(Guid? OriginalMessageId, Guid? OriginalCorrelationId);
    }
}
