namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.ActiveMqTransport.Configuration;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework.Messages;

    [TestFixture]
    public class Sending_to_a_shared_topic_subscription_endpoint :
        ActiveMqTestFixture
    {
        public Sending_to_a_shared_topic_subscription_endpoint()
            : base(new ActiveMqTestHarness { HostAddress = new Uri("amqp://localhost:61618") })
        {
        }

        [Test]
        [Category("Flaky")] //Only Artemis support shared durable topic
        public async Task Should_succeed()
        {
            var endpoint = await Bus.GetSendEndpoint(new Uri("topic:shared-durable"));
            await endpoint.Send(new PrivateMessage { Value = "Hello" });

            ConsumeContext<PrivateMessage> context = await _handler;

            Assert.That(context.Message.Value, Is.EqualTo("Hello"));
        }

        Task<ConsumeContext<PrivateMessage>> _handler;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            configurator.Bind("shared-durable", c =>
            {
                ((ConsumerConsumeTopicTopologySpecification)c).Shared = true;
                ((ConsumerConsumeTopicTopologySpecification)c).ConsumerName = "activemq-test";
            });

            _handler = Handled<PrivateMessage>(configurator);

            Handled<PingMessage>(configurator);
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.EnableArtemisCompatibility();
            base.ConfigureActiveMqBus(configurator);
        }

        class PrivateMessage
        {
            public string Value { get; set; }
        }
    }
}
