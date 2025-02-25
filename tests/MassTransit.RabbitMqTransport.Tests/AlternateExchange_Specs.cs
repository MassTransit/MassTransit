namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using TestFramework.Messages;
    using Testing;


    public class Using_alternate_exchange_with_the_outbox
    {
        [Test]
        public async Task Should_deal_with_the_alternate_exchange()
        {
            const string alternateExchangeName = "unused-message";
            const string alternateQueueName = "unused-message-queue";

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

                    x.AddHandler((UnusedMessage _) => Task.CompletedTask)
                        .Endpoint(e =>
                        {
                            e.ConfigureConsumeTopology = false;
                            e.Name = alternateQueueName;
                        });

                    x.AddConsumer<MessageHandler>()
                        .Endpoint(e => e.Name = "outbox-normal");

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                    x.AddConfigureEndpointsCallback((provider, name, cfg) =>
                    {
                        if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
                        {
                            if (name == alternateQueueName)
                            {
                                rmq.Bind(alternateExchangeName);
                            }
                        }

                        cfg.UseInMemoryInboxOutbox(provider);
                    });

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.PublishTopology.GetMessageTopology<UnusedMessage>()
                            .BindAlternateExchangeQueue(alternateExchangeName);

                        cfg.DeployPublishTopology = true;

                        cfg.ConfigureEndpoints(context);
                    });
                }).BuildServiceProvider();

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new PingMessage(), Pipe.Execute<SendContext>(context =>
            {
            }), harness.CancellationToken);

            IReceivedMessage<UnusedMessage> message = await harness.Consumed.SelectAsync<UnusedMessage>().FirstOrDefault();

            Assert.That(message, Is.Not.Null);

            // Assert.That(message.Context.TryGetPayload<RabbitMqBasicConsumeContext>(out var rmqContext), Is.True);
            // Assert.That(rmqContext.Properties.Headers.TryGetValue(), Is.EqualTo(3));
        }


        class MessageHandler :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return context.Publish(new UnusedMessage());
            }
        }


        class UnusedMessage
        {
        }
    }


    [TestFixture]
    public class AlternateExchange_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_create_and_bind_the_exchange_and_properties()
        {
            await Bus.Publish<TheWorldImploded>(new { Value = "Whoa!" });

            await _handled;
        }

        [Test]
        public async Task Should_have_the_proper_address()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Bus.Topology.TryGetPublishAddress<TheWorldImploded>(out var address));

                Assert.That(address,
                    Is.EqualTo(new Uri(
                        "rabbitmq://localhost/test/MassTransit.RabbitMqTransport.Tests:AlternateExchange_Specs-TheWorldImploded?alternateexchange=publish-not-delivered")));
            });
        }

        Task<ConsumeContext<TheWorldImploded>> _handled;

        const string AlternateExchangeName = "publish-not-delivered";
        const string AlternateQueueName = "world-examiner";

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.PublishTopology.GetMessageTopology<TheWorldImploded>()
                .BindAlternateExchangeQueue(AlternateExchangeName);

            configurator.ReceiveEndpoint(AlternateQueueName, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Bind(AlternateExchangeName);

                _handled = Handled<TheWorldImploded>(x);
            });
        }

        protected override async Task OnCleanupVirtualHost(IChannel channel)
        {
            await base.OnCleanupVirtualHost(channel);

            await channel.ExchangeDeleteAsync(AlternateExchangeName);
            await channel.QueueDeleteAsync(AlternateExchangeName);

            await channel.ExchangeDeleteAsync(AlternateQueueName);
            await channel.QueueDeleteAsync(AlternateQueueName);
        }


        public interface TheWorldImploded
        {
            string Value { get; }
        }
    }
}
