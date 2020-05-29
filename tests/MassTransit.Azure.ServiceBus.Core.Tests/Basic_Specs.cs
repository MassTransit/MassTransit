namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_a_basic_endpoint :
        AzureServiceBusTestFixture
    {
        [Test]
        [Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            var result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

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
