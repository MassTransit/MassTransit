namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware.Caching;
    using NUnit.Framework;


    [TestFixture]
    public class When_the_redelivery_header_is_present
    {
        [Test]
        public async Task Should_not_exist_on_outgoing_messages()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<InboundMessage> context) =>
                    {
                        if (context.GetRedeliveryCount() == 1)
                        {
                            await context.Publish(new OutboundMessage());
                            return;
                        }

                        throw new TestException("Ouch!");
                    });

                    x.AddHandler(async (ConsumeContext<OutboundMessage> context) =>
                    {
                    });

                    x.AddConfigureEndpointsCallback((context, name, cfg) =>
                    {
                        cfg.UseDelayedRedelivery(r => r.Interval(10, 100));
                    });
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new InboundMessage());

            IReceivedMessage<OutboundMessage> message = await harness.Consumed.SelectAsync<OutboundMessage>().Take(1).FirstOrDefault();
            Assert.That(message, Is.Not.Null);

            Assert.That(message.Context.GetHeader(MessageHeaders.RedeliveryCount, default(int?)), Is.Null);
        }


        class InboundMessage
        {
        }


        class OutboundMessage
        {
        }
    }
}
