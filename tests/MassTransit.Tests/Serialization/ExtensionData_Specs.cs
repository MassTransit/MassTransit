namespace MassTransit.Tests.Serialization;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MassTransit.Serialization;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;


[TestFixture(typeof(SystemTextJsonMessageSerializer))]
[TestFixture(typeof(SystemTextJsonRawMessageSerializer))]
public class ExtensionData_Specs
{
    [Test]
    public async Task Should_include_the_required_headers()
    {
        await using var provider = CreateServiceProvider();

        var harness = await provider.StartTestHarness();

        await harness.Bus.Publish(new ExtensiveMessage
        {
            Extra = new Dictionary<string, object>
            {
                { "text", "Value" },
                { "number", 2 }
            }
        });

        IReceivedMessage<ExtensiveMessage> message = await harness.Consumed.SelectAsync<ExtensiveMessage>().FirstOrDefault();

        Assert.That(message, Is.Not.Null);
        Assert.That(message.Exception, Is.Null);

        Assert.That(message.Context.Message.Extra.ContainsKey("text"));
        Assert.That(message.Context.Message.Extra.ContainsKey("number"));

        await harness.Stop();
    }

    readonly Type _serializerType;

    public ExtensionData_Specs(Type serializerType)
    {
        _serializerType = serializerType;
    }

    ServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ExtensiveMessageConsumer>();

                x.UsingInMemory((context, cfg) =>
                {
                    if (_serializerType == typeof(SystemTextJsonMessageSerializer))
                    {
                        cfg.ConfigureJsonSerializerOptions(options =>
                        {
                            options.SetMessageSerializerOptions<ExtensiveMessage>();

                            return options;
                        });
                    }
                    else if (_serializerType == typeof(SystemTextJsonRawMessageSerializer))
                    {
                        cfg.ClearSerialization();
                        cfg.UseRawJsonSerializer();

                        cfg.ConfigureJsonSerializerOptions(options =>
                        {
                            options.SetMessageSerializerOptions<ExtensiveMessage>();

                            return options;
                        });
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


    public class ExtensiveMessageConsumer :
        IConsumer<ExtensiveMessage>
    {
        public Task Consume(ConsumeContext<ExtensiveMessage> context)
        {
            return Task.CompletedTask;
        }
    }


    public class ExtensiveMessage
    {
        [JsonExtensionData]
        public Dictionary<string, object> Extra { get; set; }
    }
}
