namespace MassTransit.DbTransport.Tests;

using System;
using System.Threading.Tasks;
using MassTransit.Tests.Middleware.Caching;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
[TestFixture]
public class When_the_redelivery_header_is_present<T>
    where T : IDatabaseTestConfiguration, new()
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

                x.AddHandler(async (ConsumeContext<OutboundMessage> _) =>
                {
                });

                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                x.AddConfigureEndpointsCallback((_, _, cfg) =>
                {
                    cfg.UseDelayedRedelivery(r => r.Interval(10, 1000));
                });

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await harness.Bus.Publish(new InboundMessage());

        IReceivedMessage<OutboundMessage> message = await harness.Consumed.SelectAsync<OutboundMessage>().Take(1).FirstOrDefault();
        Assert.That(message, Is.Not.Null);

        Assert.That(message.Context.GetHeader(MessageHeaders.RedeliveryCount, default(int?)), Is.Null);
    }

    readonly T _configuration;

    public When_the_redelivery_header_is_present()
    {
        _configuration = new T();
    }


    class InboundMessage
    {
    }


    class OutboundMessage
    {
    }
}
