namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Initializers;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Serialization;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_raw_json_with_no_content_type :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_deserialize()
        {
            var (message, _) = await MessageInitializerCache<RawContract>.InitializeMessage(new
            {
                Name = "Frank",
                Value = 27,
                InVar.Timestamp
            });

            var body = JsonSerializer.SerializeToUtf8Bytes(message, SystemTextJsonMessageSerializer.Options);

            await SendRawMessage(body);

            ConsumeContext<RawContract> received = await _receivedA;

            Assert.Multiple(() =>
            {
                Assert.That(received.Message.Name, Is.EqualTo(message.Name));
                Assert.That(received.Message.Value, Is.EqualTo(message.Value));
                Assert.That(received.Message.Timestamp, Is.EqualTo(message.Timestamp));
            });
        }

        Task<ConsumeContext<RawContract>> _receivedA;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ClearSerialization();
            configurator.UseRawJsonSerializer(RawSerializerOptions.All);

            _receivedA = Handled<RawContract>(configurator);
        }

        async Task SendRawMessage(byte[] body)
        {
            try
            {
                await TestContext.Out.WriteLineAsync(Encoding.UTF8.GetString(body));

                var settings = GetHostSettings();
                var connectionFactory = settings.GetConnectionFactory();

                await using var connection = settings.EndpointResolver != null
                    ? await connectionFactory.CreateConnectionAsync(settings.EndpointResolver, settings.Host)
                    : await connectionFactory.CreateConnectionAsync();

                await using var channel = await connection.CreateChannelAsync();

                var properties = new BasicProperties();
                properties.SetHeader(MessageHeaders.MessageId, "Whiskey-Tango-Foxtrot 3-5-9er");

                await channel.BasicPublishAsync(RabbitMqTestHarness.InputQueueName, "", false, properties, body);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        public interface RawContract
        {
            string Name { get; }
            int Value { get; }
            DateTime Timestamp { get; }
        }
    }


    [TestFixture]
    public class Sending_raw_xml_with_no_content_type :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_deserialize()
        {
            var (message, _) = await MessageInitializerCache<RawContract>.InitializeMessage(new
            {
                Name = "Frank",
                Value = 27,
                InVar.Timestamp
            });

            using var ms = new MemoryStream(4000);
            NewtonsoftXmlMessageSerializer.Serialize(ms, message, typeof(RawContract));

            var body = ms.ToArray();

            await SendRawMessage(body);

            ConsumeContext<RawContract> received = await _receivedA;

            Assert.Multiple(() =>
            {
                Assert.That(received.Message.Name, Is.EqualTo(message.Name));
                Assert.That(received.Message.Value, Is.EqualTo(message.Value));
                Assert.That(received.Message.Timestamp, Is.EqualTo(message.Timestamp));
            });
        }

        Task<ConsumeContext<RawContract>> _receivedA;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ClearSerialization();
            configurator.UseRawXmlSerializer();

            _receivedA = Handled<RawContract>(configurator);
        }

        async Task SendRawMessage(byte[] body)
        {
            try
            {
                await TestContext.Out.WriteLineAsync(Encoding.UTF8.GetString(body));

                var settings = GetHostSettings();
                var connectionFactory = settings.GetConnectionFactory();

                await using var connection = settings.EndpointResolver != null
                    ? await connectionFactory.CreateConnectionAsync(settings.EndpointResolver, settings.Host)
                    : await connectionFactory.CreateConnectionAsync();

                await using var channel = await connection.CreateChannelAsync();

                var properties = new BasicProperties();
                properties.SetHeader(MessageHeaders.MessageId, "Whiskey-Tango-Foxtrot 3-5-9er");

                await channel.BasicPublishAsync(RabbitMqTestHarness.InputQueueName, "", false, properties, body);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        public interface RawContract
        {
            string Name { get; }
            int Value { get; }
            DateTime Timestamp { get; }
        }
    }


    [TestFixture]
    public class Sending_and_consuming_raw_json_with_headers :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_return_the_header_value_from_the_transport()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27"
            };

            const string headerName = "Random-Header";
            const string headerValue = "SomeValue";

            await InputQueueSendEndpoint.Send(message, x => x.Headers.Set(headerName, headerValue));

            ConsumeContext<Command> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(SystemTextJsonRawMessageSerializer.JsonContentType),
                    $"unexpected content-type {context.ReceiveContext.ContentType}");

                Assert.That(context.Message.CommandId, Is.EqualTo(message.CommandId));
                Assert.That(context.Message.ItemNumber, Is.EqualTo(message.ItemNumber));

                Assert.That(context.Headers.Get<string>(headerName), Is.EqualTo(headerValue));

                Assert.That(context.MessageId.HasValue, Is.True);
                Assert.That(context.ConversationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.SentTime.HasValue, Is.True);
                Assert.That(context.DestinationAddress, Is.Not.Null);
                Assert.That(context.Host, Is.Not.Null);
                Assert.That(context.SupportedMessageTypes.Count(), Is.EqualTo(1));
            });
        }

        Task<ConsumeContext<Command>> _handled;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseRawJsonSerializer(RawSerializerOptions.All);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<Command>(configurator);
        }


        public interface Command
        {
            Guid CommandId { get; }
            string ItemNumber { get; }
        }


        public class BagOfCrap
        {
            public Guid CommandId { get; set; }
            public string ItemNumber { get; set; }
        }
    }


    [TestFixture]
    public class Sending_and_consuming_raw_json_with_headers_and_producing :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_not_forward_transport_headers_from_raw_json()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27"
            };

            const string headerName = "Random-Header";
            const string headerValue = "SomeValue";

            await InputQueueSendEndpoint.Send(message, x =>
            {
                x.Headers.Set(headerName, headerValue);
                x.Serializer = new NewtonsoftRawJsonMessageSerializer();
            });

            ConsumeContext<Command> commandContext = await _handler;

            Assert.That(commandContext.ReceiveContext.ContentType, Is.EqualTo(NewtonsoftRawJsonMessageSerializer.RawJsonContentType),
                $"unexpected content-type {commandContext.ReceiveContext.ContentType}");

            ConsumeContext<PingMessage> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(NewtonsoftJsonMessageSerializer.JsonContentType),
                    $"unexpected content-type {context.ReceiveContext.ContentType}");

                Assert.That(context.Message.CorrelationId, Is.EqualTo(message.CommandId));

                Assert.That(context.Headers.Get<string>(headerName), Is.EqualTo((string)default));
            });
        }

        Task<ConsumeContext<PingMessage>> _handled;
        Task<ConsumeContext<Command>> _handler;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("second-queue", e =>
            {
                _handled = Handled<PingMessage>(e);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.UseNewtonsoftRawJsonDeserializer(RawSerializerOptions.AnyMessageType);

            TaskCompletionSource<ConsumeContext<Command>> handler = GetTask<ConsumeContext<Command>>();
            _handler = handler.Task;

            Handler<Command>(configurator, async context =>
            {
                await context.Publish(new PingMessage(context.Message.CommandId));

                handler.SetResult(context);
            });
        }


        public interface Command
        {
            Guid CommandId { get; }
            string ItemNumber { get; }
        }


        public class BagOfCrap
        {
            public Guid CommandId { get; set; }
            public string ItemNumber { get; set; }
        }
    }
}
