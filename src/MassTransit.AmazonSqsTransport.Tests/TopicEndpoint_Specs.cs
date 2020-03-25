namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using NUnit.Framework;


    [TestFixture]
    public class Sending_to_a_topic_endpoint :
        AmazonSqsTestFixture
    {
        Task<ConsumeContext<PrivateMessage>> _handler;

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            configurator.Subscribe("private-topic", s =>
            {
            });

            _handler = Handled<PrivateMessage>(configurator);
        }

        [Test]
        public async Task Should_succeed()
        {
            var endpoint = await Bus.GetSendEndpoint(new Uri("topic:private-topic"));
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
