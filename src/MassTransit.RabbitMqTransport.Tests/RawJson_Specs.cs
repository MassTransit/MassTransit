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
            RawContract contract = await MessageInitializerCache<RawContract>.InitializeMessage(new
            {
                Name = "Frank",
                Value = 27,
                InVar.Timestamp
            });

            var jsonText = JsonConvert.SerializeObject(contract, JsonMessageSerializer.SerializerSettings);
            var body = Encoding.UTF8.GetBytes(jsonText);

            SendRawMessage(Host, body);

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

        void SendRawMessage(IRabbitMqHost host, byte[] body)
        {
            try
            {
                var connectionFactory = host.Settings.GetConnectionFactory();

                using var connection = host.Settings.EndpointResolver != null
                    ? connectionFactory.CreateConnection(host.Settings.EndpointResolver, host.Settings.Host)
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
}
