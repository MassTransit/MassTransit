namespace MassTransit.Tests.ContainerTests
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

#if NET8_0_OR_GREATER


    public class Using_the_date_only_time_only_property_types
    {
        [Test]
        public async Task Should_handle_the_message_as_a_consumer()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyMessage message) =>
                    {
                        Assert.That(message, Is.Not.Null);
                    });

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureJsonSerializerOptions(opts =>
                        {
                            opts.Converters.Add(new DateOnlyJsonConverter());
                            opts.Converters.Add(new TimeOnlyJsonConverter());
                            return opts;
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var dateOnly = DateOnly.FromDateTime(DateTimeOffset.Now.DateTime);
            var timeOnly = TimeOnly.FromDateTime(DateTimeOffset.Now.DateTime);

            await harness.Bus.Publish(new MyMessage
            {
                Date = dateOnly,
                Time = timeOnly
            });

            Assert.That(await harness.Consumed.Any<MyMessage>(), Is.True);

            IReceivedMessage<MyMessage> message = await harness.Consumed.SelectAsync<MyMessage>().FirstOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(message.Context.Message.Date, Is.EqualTo(dateOnly));
                Assert.That(message.Context.Message.Time, Is.EqualTo(timeOnly));
            });
        }


        public record MyMessage
        {
            public DateOnly Date { get; set; }
            public TimeOnly Time { get; set; }
        }
    }


    public class TimeOnlyJsonConverter :
        JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeOnly.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            var isoTime = value.ToString("O");
            writer.WriteStringValue(isoTime);
        }
    }


    public class DateOnlyJsonConverter :
        JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateOnly.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            var isoDate = value.ToString("O");
            writer.WriteStringValue(isoDate);
        }
    }
#endif
}
