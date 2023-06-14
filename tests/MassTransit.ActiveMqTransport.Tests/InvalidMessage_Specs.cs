namespace MassTransit.ActiveMqTransport.Tests
{
    using System.Threading.Tasks;
    using Apache.NMS;
    using NUnit.Framework;


    [TestFixture]
    public class InvalidMessage_Specs :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_fault()
        {
            Task<ConsumeContext<ReceiveFault>> receiveFault = await ConnectPublishHandler<ReceiveFault>();

            await ProduceInvalidMessage();

            await receiveFault;
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            Handled<SubmitOrder>(configurator);
        }

        async Task ProduceInvalidMessage()
        {
            var options = new ActiveMqTransportOptions();

            var brokerAddress = $"activemq:tcp://{options.Host}:{options.Port}";

            var factory = new NMSConnectionFactory(brokerAddress);

            var connection = factory.ConnectionFactory.CreateConnection(options.User, options.Pass);
            try
            {
                var session = connection.CreateSession(AcknowledgementMode.ClientAcknowledge);

                var producer = session.CreateProducer(session.GetQueue(ActiveMqTestHarness.InputQueueName));

                var message = session.CreateMessage();

                message.NMSCorrelationID = "AB76E632-8550-49B9-A119-BBEB84D53355";

                await producer.SendAsync(message);

                await producer.CloseAsync();
                session.Close();
            }
            finally
            {
                connection.Close();
            }
        }


        class SubmitOrder
        {
            public string OrderId { get; set; }
        }
    }
}
