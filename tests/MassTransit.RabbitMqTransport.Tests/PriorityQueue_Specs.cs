namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class Using_a_priority_queue :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_allow_priority_to_be_specified()
        {
            Response<PongMessage> response = await Bus.Request<PingMessage, PongMessage>(_endpointAddress, new PingMessage(), TestCancellationToken,
                TestTimeout, x =>
                {
                    x.SetPriority(2);
                });

            Assert.That(response.Headers.Get<string>("Received-Priority"), Is.EqualTo("2"));
        }

        Uri _endpointAddress;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("priority_input_queue", x =>
            {
                x.ConfigureConsumeTopology = false;

                x.EnablePriority(4);

                _endpointAddress = x.InputAddress;

                x.Handler<PingMessage>(context =>
                {
                    return context.RespondAsync(new PongMessage(context.Message.CorrelationId), sc =>
                    {
                        sc.Headers.Set("Received-Priority", context.GetPayload<RabbitMqBasicConsumeContext>().Properties.Priority.ToString());
                    });
                });
            });
        }

        protected override async Task OnCleanupVirtualHost(IChannel channel)
        {
            await base.OnCleanupVirtualHost(channel);

            await channel.ExchangeDeleteAsync("priority_input_queue");
            await channel.QueueDeleteAsync("priority_input_queue");
        }
    }


    [TestFixture]
    public class Using_priority_queues_with_outbox
    {
        [Test]
        public async Task Should_retain_the_priority()
        {
            await using var provider = new ServiceCollection()
                .ConfigureRabbitMqTestOptions(options =>
                {
                    options.CleanVirtualHost = true;
                    options.CreateVirtualHostIfNotExists = true;
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddOptions<RabbitMqTransportOptions>()
                        .Configure(options => options.VHost = "test");

                    x.AddInMemoryInboxOutbox();

                    x.AddHandler((BusMessage _) => Task.CompletedTask)
                        .Endpoint(e => e.Name = "observer-priority");

                    x.AddConsumer<MessageHandler>()
                        .Endpoint(e => e.Name = "outbox-priority");

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                    x.AddConfigureEndpointsCallback((provider, name, cfg) =>
                    {
                        if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
                            rmq.EnablePriority(5);

                        cfg.UseInMemoryInboxOutbox(provider);
                    });

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();

            await harness.Start();

            var correlationId = NewId.NextGuid();
            var conversationId = NewId.NextGuid();
            var initiatorId = NewId.NextGuid();
            var messageId = NewId.NextGuid();
            await harness.Bus.Publish(new PingMessage(), Pipe.Execute<SendContext>(context =>
            {
                context.SetPriority(2);

                context.CorrelationId = correlationId;
                context.MessageId = messageId;
                context.InitiatorId = initiatorId;
                context.ConversationId = conversationId;
            }), harness.CancellationToken);

            IReceivedMessage<BusMessage> message = await harness.Consumed.SelectAsync<BusMessage>().FirstOrDefault();

            Assert.That(message, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(message.Context.TryGetPayload<RabbitMqBasicConsumeContext>(out var rmqContext), Is.True);
                Assert.That(rmqContext.Properties.Priority, Is.EqualTo(3));
            });
        }


        class MessageHandler :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return context.Publish(new BusMessage
                {
                    OriginalMessageId = context.MessageId,
                    OriginalCorrelationId = context.CorrelationId
                }, sendContext => sendContext.SetPriority(3));
            }
        }


        class BusMessage
        {
            public Guid? OriginalMessageId { get; set; }
            public Guid? OriginalCorrelationId { get; set; }
        }
    }
}
