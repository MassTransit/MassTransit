namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_to_a_virtual_topic_endpoint :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            var endpoint = await Bus.GetSendEndpoint(new Uri("topic:VirtualTopic.private"));
            await endpoint.Send(new PrivateMessage { Value = "Hello" });

            ConsumeContext<PrivateMessage> context = await _handler;

            Assert.That(context.Message.Value, Is.EqualTo("Hello"));
        }

        Task<ConsumeContext<PrivateMessage>> _handler;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            configurator.Bind("VirtualTopic.private");

            _handler = Handled<PrivateMessage>(configurator);

            Handled<PingMessage>(configurator);
        }


        class PrivateMessage
        {
            public string Value { get; set; }
        }
    }
}
