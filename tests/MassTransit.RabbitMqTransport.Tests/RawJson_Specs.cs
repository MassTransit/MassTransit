namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Initializers;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Serialization;


    [TestFixture]
    public class Sending_raw_json_with_no_content_type :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_deserialize()
        {
            var contract = await MessageInitializerCache<RawContract>.InitializeMessage(new
            {
                Name = "Frank",
                Value = 27,
                InVar.Timestamp
            });

            var jsonText = JsonConvert.SerializeObject(contract, JsonMessageSerializer.SerializerSettings);
            byte[] body = Encoding.UTF8.GetBytes(jsonText);

            SendRawMessage(body);

            ConsumeContext<RawContract> received = await _receivedA;

            Assert.AreEqual(contract.Name, received.Message.Name);
            Assert.AreEqual(contract.Value, received.Message.Value);
            Assert.AreEqual(contract.Timestamp, received.Message.Timestamp);
        }

        Task<ConsumeContext<RawContract>> _receivedA;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ClearMessageDeserializers();
            configurator.UseRawJsonSerializer();

            _receivedA = Handled<RawContract>(configurator);
        }

        void SendRawMessage(byte[] body)
        {
            try
            {
                var settings = GetHostSettings();
                var connectionFactory = settings.GetConnectionFactory();

                using var connection = settings.EndpointResolver != null
                    ? connectionFactory.CreateConnection(settings.EndpointResolver, settings.Host)
                    : connectionFactory.CreateConnection();

                using var model = connection.CreateModel();

                var properties = model.CreateBasicProperties();

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

            Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(RawJsonMessageSerializer.RawJsonContentType),
                $"unexpected content-type {context.ReceiveContext.ContentType}");

            Assert.That(context.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(context.Message.ItemNumber, Is.EqualTo(message.ItemNumber));

            Assert.That(context.Headers.Get<string>(headerName), Is.EqualTo(headerValue));
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
}
