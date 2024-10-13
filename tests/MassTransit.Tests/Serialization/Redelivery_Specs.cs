namespace MassTransit.Tests.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Internals;
using MassTransit.Serialization;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework;


[TestFixture(typeof(SystemTextJsonMessageSerializer))]
[TestFixture(typeof(SystemTextJsonRawMessageSerializer))]
[TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
[TestFixture(typeof(NewtonsoftRawJsonMessageSerializer))]
[TestFixture(typeof(MessagePackMessageSerializer))]
public class Redelivery_Specs
{
    [Test]
    public async Task Should_include_the_required_headers()
    {
        await using var provider = CreateServiceProvider();

        var harness = await provider.StartTestHarness();

        await harness.Bus.Publish(new FaultyMessage());

        Assert.That(await harness.Published.Any<FinalMessage>());

        IList<IReceivedMessage<FaultyMessage>> messages = await harness.Consumed.SelectAsync<FaultyMessage>().Take(2).ToListAsync();

        IReceivedMessage<FaultyMessage> faulted = messages.First();
        Assert.That(faulted, Is.Not.Null);
        Assert.That(faulted.Context.SupportedMessageTypes, Does.Contain(MessageUrn.ForTypeString<FaultyMessage>()));

        IReceivedMessage<FaultyMessage> consumed = messages.Last();

        Assert.That(consumed, Is.Not.Null);
        Assert.That(consumed.Context.SupportedMessageTypes, Does.Contain(MessageUrn.ForTypeString<FaultyMessage>()));

        await harness.Stop();
    }

    ServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<FaultyConsumer>();

                x.AddConfigureEndpointsCallback((provider, name, cfg) =>
                {
                    cfg.UseDelayedRedelivery(r =>
                    {
                        r.Intervals(5, 10);
                        r.ReplaceMessageId = true;
                    });
                });

                x.UsingInMemory((context, cfg) =>
                {
                    if (_serializerType == typeof(SystemTextJsonMessageSerializer))
                    {
                    }
                    else if (_serializerType == typeof(SystemTextJsonRawMessageSerializer))
                    {
                        cfg.ClearSerialization();
                        cfg.UseRawJsonSerializer();
                    }
                    else if (_serializerType == typeof(NewtonsoftJsonMessageSerializer))
                    {
                        cfg.ClearSerialization();
                        cfg.UseNewtonsoftJsonSerializer();
                    }
                    else if (_serializerType == typeof(NewtonsoftRawJsonMessageSerializer))
                    {
                        cfg.ClearSerialization();
                        cfg.UseNewtonsoftRawJsonSerializer();
                    }

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);
    }

    readonly Type _serializerType;

    public Redelivery_Specs(Type serializerType)
    {
        _serializerType = serializerType;
    }


    class FaultyConsumer :
        IConsumer<FaultyMessage>
    {
        public async Task Consume(ConsumeContext<FaultyMessage> context)
        {
            if (context.GetRedeliveryCount() == 0)
                throw new IntentionalTestException();

            await context.Publish(new FinalMessage());
        }
    }


    record FaultyMessage
    {
    }

    record FinalMessage
    {
    }
}
