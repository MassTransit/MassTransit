namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    [Category("Flaky")]
    public class When_using_a_name_property_for_correlation :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_handle_named_property()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<OtherMessage>(new { CorrelationId = transactionId });

            ConsumeContext<OtherMessage> otherContext = await _otherHandled;

            Assert.Multiple(() =>
            {
                Assert.That(otherContext.CorrelationId.HasValue, Is.True);
                Assert.That(otherContext.CorrelationId.Value, Is.EqualTo(transactionId));
            });
        }

        Task<ConsumeContext<OtherMessage>> _otherHandled;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _otherHandled = Handled<OtherMessage>(configurator);
        }


        public class OtherMessage
        {
            public Guid CorrelationId { get; set; }
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class When_using_named_legacy_config :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_handle_named_configured_legacy()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<LegacyMessage>(new { TransactionId = transactionId });

            ConsumeContext<LegacyMessage> legacyContext = await _legacyHandled;

            Assert.Multiple(() =>
            {
                Assert.That(legacyContext.CorrelationId.HasValue, Is.True);
                Assert.That(legacyContext.CorrelationId.Value, Is.EqualTo(transactionId));
            });
        }

        public When_using_named_legacy_config()
        {
            MessageCorrelation.UseCorrelationId<LegacyMessage>(x => x.TransactionId);
        }

        Task<ConsumeContext<LegacyMessage>> _legacyHandled;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _legacyHandled = Handled<LegacyMessage>(configurator);
        }


        public class LegacyMessage
        {
            public Guid TransactionId { get; set; }
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class When_using_a_base_event :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_handle_base_event_class()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<INewUserEvent>(new { TransactionId = transactionId });

            ConsumeContext<INewUserEvent> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(transactionId));
            });
        }

        Task<ConsumeContext<INewUserEvent>> _handled;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Send<IEvent>(x =>
            {
                x.UseCorrelationId(p => p.TransactionId);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<INewUserEvent>(configurator);
        }


        public interface IEvent
        {
            Guid TransactionId { get; }
        }


        public interface INewUserEvent :
            IEvent
        {
        }
    }
}
