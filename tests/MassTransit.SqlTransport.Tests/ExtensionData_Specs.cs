namespace MassTransit.DbTransport.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Using_json_extension_data<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_properly_serialize_the_message()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(TextWriter.Null, x =>
            {
                x.AddConsumer<ExtensionConsumer>();
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.ConfigureJsonSerializerOptions(options =>
                    {
                        options.SetMessageSerializerOptions<WithObject>();
                        options.SetMessageSerializerOptions<WithElement>();

                        return options;
                    });
                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        await harness.Bus.Publish(new WithElement
        {
            Extra = new Dictionary<string, JsonElement>
            {
                { "text", JsonSerializer.SerializeToElement("Value") },
                { "number", JsonSerializer.SerializeToElement(2) }
            }
        });

        IReceivedMessage<WithElement> message = await harness.Consumed.SelectAsync<WithElement>().FirstOrDefault();

        Assert.That(message, Is.Not.Null);
        Assert.That(message.Exception, Is.Null);

        Assert.That(message.Context.Message.Extra.ContainsKey("text"));
        Assert.That(message.Context.Message.Extra.ContainsKey("number"));

        await harness.Bus.Publish(new WithObject
        {
            Extra = new Dictionary<string, object>
            {
                { "text", "Value" },
                { "number", 2 }
            }
        });

        IReceivedMessage<WithObject> m2 = await harness.Consumed.SelectAsync<WithObject>().FirstOrDefault();

        Assert.That(m2, Is.Not.Null);
        Assert.That(m2.Exception, Is.Null);

        Assert.That(m2.Context.Message.Extra.ContainsKey("text"));
        Assert.That(m2.Context.Message.Extra.ContainsKey("number"));
    }

    readonly T _configuration;


    class ExtensionConsumer :
        IConsumer<WithObject>,
        IConsumer<WithElement>
    {
        public Task Consume(ConsumeContext<WithElement> context)
        {
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<WithObject> context)
        {
            return Task.CompletedTask;
        }
    }


    public Using_json_extension_data()
    {
        _configuration = new T();
    }


    public record WithObject
    {
        [JsonExtensionData]
        public Dictionary<string, object> Extra { get; set; }
    }


    public record WithElement
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }
}
