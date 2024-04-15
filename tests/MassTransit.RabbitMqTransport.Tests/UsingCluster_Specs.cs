namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    [Category("Flaky")]
    public class When_clustering_nodes_into_a_logical_broker :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_use_the_logical_host_name()
        {
            var message = new PingMessage();
            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<PingMessage> received = await _receivedA;

            Assert.Multiple(() =>
            {
                Assert.That(received.Message.CorrelationId, Is.EqualTo(message.CorrelationId));

                Assert.That(received.DestinationAddress.Host, Is.EqualTo(_logicalHostAddress.Host));
            });
        }

        public When_clustering_nodes_into_a_logical_broker()
            : base(new Uri("rabbitmq://cluster/test/"))
        {
        }

        readonly Uri _logicalHostAddress = new Uri("rabbitmq://cluster/test/");

        Task<ConsumeContext<PingMessage>> _receivedA;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _receivedA = Handled<PingMessage>(configurator);
        }
    }
}
