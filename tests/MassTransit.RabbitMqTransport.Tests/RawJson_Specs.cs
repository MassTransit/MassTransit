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

            SendRawMessage(body);

            ConsumeContext<RawContract> received = await _receivedA;

            Assert.AreEqual(message.Name, received.Message.Name);
            Assert.AreEqual(message.Value, received.Message.Value);
            Assert.AreEqual(message.Timestamp, received.Message.Timestamp);
        }

        Task<ConsumeContext<RawContract>> _receivedA;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ClearSerialization();
            configurator.UseRawJsonSerializer();

            _receivedA = Handled<RawContract>(configurator);
        }

        void SendRawMessage(byte[] body)
        {
            try
            {
                TestContext.Out.WriteLine(Encoding.UTF8.GetString(body));

                var settings = GetHostSettings();
                var connectionFactory = settings.GetConnectionFactory();

                using var connection = settings.EndpointResolver != null
                    ? connectionFactory.CreateConnection(settings.EndpointResolver, settings.Host)
                    : connectionFactory.CreateConnection();

                using var model = connection.CreateModel();

                var properties = model.CreateBasicProperties();
                properties.SetHeader(MessageHeaders.MessageId, "Whiskey-Tango-Foxtrot 3-5-9er");

                model.BasicPublish(RabbitMqTestHarness.InputQueueName, "", false, properties, body);
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

            await using var ms = new MemoryStream(4000);
            NewtonsoftXmlMessageSerializer.Serialize(ms, message, typeof(RawContract));

            var body = ms.ToArray();

            SendRawMessage(body);

            ConsumeContext<RawContract> received = await _receivedA;

            Assert.AreEqual(message.Name, received.Message.Name);
            Assert.AreEqual(message.Value, received.Message.Value);
            Assert.AreEqual(message.Timestamp, received.Message.Timestamp);
        }

        Task<ConsumeContext<RawContract>> _receivedA;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ClearSerialization();
            configurator.UseRawXmlSerializer();

            _receivedA = Handled<RawContract>(configurator);
        }

        void SendRawMessage(byte[] body)
        {
            try
            {
                TestContext.Out.WriteLine(Encoding.UTF8.GetString(body));

                var settings = GetHostSettings();
                var connectionFactory = settings.GetConnectionFactory();

                using var connection = settings.EndpointResolver != null
                    ? connectionFactory.CreateConnection(settings.EndpointResolver, settings.Host)
                    : connectionFactory.CreateConnection();

                using var model = connection.CreateModel();

                var properties = model.CreateBasicProperties();
                properties.SetHeader(MessageHeaders.MessageId, "Whiskey-Tango-Foxtrot 3-5-9er");

                model.BasicPublish(RabbitMqTestHarness.InputQueueName, "", false, properties, body);
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

            Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(SystemTextJsonRawMessageSerializer.JsonContentType),
                $"unexpected content-type {context.ReceiveContext.ContentType}");

            Assert.That(context.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(context.Message.ItemNumber, Is.EqualTo(message.ItemNumber));

            Assert.That(context.Headers.Get<string>(headerName), Is.EqualTo(headerValue));

            Assert.IsTrue(context.MessageId.HasValue);
            Assert.IsTrue(context.ConversationId.HasValue);
            Assert.IsTrue(context.CorrelationId.HasValue);
            Assert.IsTrue(context.SentTime.HasValue);
            Assert.IsNotNull(context.DestinationAddress);
            Assert.IsNotNull(context.Host);
            Assert.That(context.SupportedMessageTypes.Count(), Is.EqualTo(1));
        }

        Task<ConsumeContext<Command>> _handled;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseRawJsonSerializer();
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

            Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(NewtonsoftJsonMessageSerializer.JsonContentType),
                $"unexpected content-type {context.ReceiveContext.ContentType}");

            Assert.That(context.Message.CorrelationId, Is.EqualTo(message.CommandId));

            Assert.That(context.Headers.Get<string>(headerName), Is.EqualTo(default));
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
            configurator.UseNewtonsoftRawJsonDeserializer();

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
