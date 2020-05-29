namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using NUnit.Framework;


    [TestFixture]
    public class TopologyCorrelationId_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_handle_base_event_class()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<INewUserEvent>(new {TransactionId = transactionId});

            ConsumeContext<INewUserEvent> context = await _handled;

            Assert.IsTrue(context.CorrelationId.HasValue);
            Assert.That(context.CorrelationId.Value, Is.EqualTo(transactionId));
        }

        [Test]
        public async Task Should_handle_named_configured_legacy()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<LegacyMessage>(new {TransactionId = transactionId});

            ConsumeContext<LegacyMessage> legacyContext = await _legacyHandled;

            Assert.IsTrue(legacyContext.CorrelationId.HasValue);
            Assert.That(legacyContext.CorrelationId.Value, Is.EqualTo(transactionId));
        }

        [Test]
        public async Task Should_handle_named_property()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<OtherMessage>(new {CorrelationId = transactionId});

            ConsumeContext<OtherMessage> otherContext = await _otherHandled;

            Assert.IsTrue(otherContext.CorrelationId.HasValue);
            Assert.That(otherContext.CorrelationId.Value, Is.EqualTo(transactionId));
        }

        Task<ConsumeContext<INewUserEvent>> _handled;
        Task<ConsumeContext<OtherMessage>> _otherHandled;
        Task<ConsumeContext<LegacyMessage>> _legacyHandled;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            MessageCorrelation.UseCorrelationId<LegacyMessage>(x => x.TransactionId);

            configurator.Send<IEvent>(x =>
            {
                x.UseCorrelationId(p => p.TransactionId);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<INewUserEvent>(configurator);
            _otherHandled = Handled<OtherMessage>(configurator);
            _legacyHandled = Handled<LegacyMessage>(configurator);
        }


        public interface IEvent
        {
            Guid TransactionId { get; }
        }


        public interface INewUserEvent :
            IEvent
        {
        }


        public class OtherMessage
        {
            public Guid CorrelationId { get; set; }
        }


        public class LegacyMessage
        {
            public Guid TransactionId { get; set; }
        }
    }
}
