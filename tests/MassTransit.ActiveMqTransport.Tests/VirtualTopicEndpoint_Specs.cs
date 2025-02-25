namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture(ActiveMqHostAddress.ActiveMqScheme)]
    [TestFixture(ActiveMqHostAddress.AmqpScheme)]
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

        public Sending_to_a_virtual_topic_endpoint(string protocol)
            : base(protocol)
        {
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PrivateMessage>> _handler;
        #pragma warning restore NUnit1032

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
