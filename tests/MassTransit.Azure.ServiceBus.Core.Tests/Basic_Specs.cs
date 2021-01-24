namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_a_basic_endpoint :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _handler;
        }

        public Sending_a_message_to_a_basic_endpoint()
            : base("input_queue", AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace, "MassTransit.Azure.ServiceBus.Core.Tests"),
                new BasicAzureServiceBusAccountSettings())
        {
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);

            configurator.SelectBasicTier();
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            base.ConfigureServiceBusBus(configurator);

            configurator.SelectBasicTier();
        }
    }
}
