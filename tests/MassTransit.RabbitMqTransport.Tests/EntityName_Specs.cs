namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Category("Flaky")]
    public class When_an_entity_name_is_specified_on_the_message_type :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await Bus.Publish<CustomEntityMessage>(new { Value = "Yawn" });

            ConsumeContext<CustomEntityMessage> received = await _receivedA;

            Assert.That(received.DestinationAddress, Is.EqualTo(new Uri(HostAddress, EntityName)));
        }

        Task<ConsumeContext<CustomEntityMessage>> _receivedA;
        const string EntityName = "custom-message";

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Message<CustomEntityMessage>(x =>
            {
                x.SetEntityName(EntityName);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _receivedA = Handled<CustomEntityMessage>(configurator);
        }


        public interface CustomEntityMessage
        {
            string Value { get; }
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class When_an_entity_name_is_specified_on_the_message_by_attribute :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await Bus.Publish<AttributeSpecifiedEntityMessage>(new { Value = "Yawn" });

            ConsumeContext<AttributeSpecifiedEntityMessage> received = await _receivedA;

            Assert.That(received.DestinationAddress, Is.EqualTo(new Uri(HostAddress, EntityName)));
        }

        Task<ConsumeContext<AttributeSpecifiedEntityMessage>> _receivedA;
        const string EntityName = "custom-entity-message";

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _receivedA = Handled<AttributeSpecifiedEntityMessage>(configurator);
        }


        [EntityName(EntityName)]
        public interface AttributeSpecifiedEntityMessage
        {
            string Value { get; }
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class When_an_entity_name_is_specified_on_the_message_by_attribute_and_a_fault_occurs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            Task<ConsumeContext<Fault<AttributeSpecifiedEntityMessage>>> faulted = await ConnectPublishHandler<Fault<AttributeSpecifiedEntityMessage>>();

            await Bus.Publish<AttributeSpecifiedEntityMessage>(new { Value = "Yawn" });

            ConsumeContext<Fault<AttributeSpecifiedEntityMessage>> received = await faulted;

            Assert.That(received.DestinationAddress, Is.EqualTo(new Uri(HostAddress, FaultEntityName)));
        }

        const string EntityName = "custom-entity-message";
        const string FaultEntityName = "fault-custom-entity-message";

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            Handler<AttributeSpecifiedEntityMessage>(configurator, context => throw new IntentionalTestException());
        }


        [EntityName(EntityName)]
        [FaultEntityName(FaultEntityName)]
        public interface AttributeSpecifiedEntityMessage
        {
            string Value { get; }
        }
    }
}
