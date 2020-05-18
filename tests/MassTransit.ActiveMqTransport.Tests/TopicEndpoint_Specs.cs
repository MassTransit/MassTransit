namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using ActiveMqTransport;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_to_a_topic_endpoint :
        ActiveMqTestFixture
    {
        Task<ConsumeContext<PrivateMessage>> _handler;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            configurator.Bind("VirtualTopic.private");

            _handler = Handled<PrivateMessage>(configurator);

            Handled<PingMessage>(configurator);
        }

        [Test]
        public async Task Should_succeed()
        {
            var endpoint = await Bus.GetSendEndpoint(new Uri("topic:VirtualTopic.private"));
            await endpoint.Send(new PrivateMessage() {Value = "Hello"});

            var context = await _handler;

            Assert.That(context.Message.Value, Is.EqualTo("Hello"));
        }


        class PrivateMessage
        {
            public string Value { get; set; }
        }
    }
}
