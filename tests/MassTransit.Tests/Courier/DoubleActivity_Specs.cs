#nullable enable
namespace MassTransit.Tests.Courier
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using DoubleActivity;
    using MassTransit.Courier.Contracts;
    using MassTransit.Serialization;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    namespace DoubleActivity
    {
        using System;
        using System.Text.Json;
        using System.Text.Json.Serialization;
        using System.Threading.Tasks;


        public class PointActivityArguments
        {
            public Point Point { get; set; } = new Point();
        }


        public class PointActivity : IExecuteActivity<PointActivityArguments>
        {
            public Task<ExecutionResult> Execute(ExecuteContext<PointActivityArguments> context)
            {
                Console.WriteLine("Received point: " + context.Arguments.Point.X + "/" + context.Arguments.Point.Y);
                return Task.FromResult(context.Completed());
            }
        }


        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }
        }


        public class PointConverter : JsonConverter<Point>
        {
            public override Point? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException("Unexpected token, expected an object with x,y properties");

                var originalDepth = reader.CurrentDepth;

                if (reader.TokenType == JsonTokenType.Null)
                {
                    reader.Read();
                    return null;
                }

                reader.Read();

                var x = double.NaN;
                var y = double.NaN;

                while (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var name = reader.GetString()?.ToLower();
                    reader.Read();

                    try
                    {
                        switch (name)
                        {
                            case "x":
                                if (reader.TokenType == JsonTokenType.Number)
                                    x = reader.GetDouble();
                                else
                                    throw new JsonException($"{name.ToUpper()}-component was no number!");
                                break;
                            case "y":
                                if (reader.TokenType == JsonTokenType.Number)
                                    y = reader.GetDouble();
                                else
                                    throw new JsonException($"{name.ToUpper()}-component was no number!");
                                break;
                        }

                        reader.Read();
                    }
                    catch (Exception e) when (e is InvalidOperationException || e is FormatException)
                    {
                        throw new JsonException($"{name?.ToUpper()}-component of the coordinate was missing or malformatted");
                    }
                }

                while (reader.TokenType != JsonTokenType.EndObject || reader.CurrentDepth != originalDepth)
                    reader.Read();

                if (double.IsNaN(x))
                    throw new JsonException("X-component of the coordiante was missing or malformatted");

                if (double.IsNaN(y))
                    throw new JsonException("Y-component of the coordiante was missing or malformatted");

                return new Point
                {
                    X = x,
                    Y = y
                };
            }

            public override void Write(Utf8JsonWriter writer, Point? value, JsonSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNullValue();
                    return;
                }

                writer.WriteStartObject();
                writer.WriteNumber("x", value.X);
                writer.WriteNumber("y", value.Y);
                writer.WriteEndObject();
            }
        }
    }


    [TestFixture]
    public class DoubleActivity_Specs
    {
        [Test]
        public async Task Should_properly_deserialize_the_juicy_double()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetKebabCaseEndpointNameFormatter();
                    x.AddExecuteActivity<PointActivity, PointActivityArguments>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddActivity(
                nameof(PointActivity),
                new Uri("queue:point_execute"),
                new PointActivityArguments
                {
                    Point = new Point
                    {
                        X = 1.2,
                        Y = 2.3
                    }
                });

            var routingSlip = builder.Build();

            await harness.Bus.Execute(routingSlip);

            Assert.That(await harness.Published.Any<RoutingSlipCompleted>());
        }

        [SetUp]
        public void Setup()
        {
            _options = SystemTextJsonMessageSerializer.Options;

            SystemTextJsonMessageSerializer.Options = new JsonSerializerOptions(SystemTextJsonMessageSerializer.Options);
            SystemTextJsonMessageSerializer.Options.Converters.Add(new PointConverter());
        }

        [TearDown]
        public void Teardown()
        {
            if (_options != null)
                SystemTextJsonMessageSerializer.Options = _options;
        }

        JsonSerializerOptions? _options;
    }
}
