namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class TopologyCorrelationId_Specs :
        AzureServiceBusTestFixture
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

        Task<ConsumeContext<INewUserEvent>> _handled;
        Task<ConsumeContext<OtherMessage>> _otherHandled;
        Task<ConsumeContext<LegacyMessage>> _legacyHandled;

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            MessageCorrelation.UseCorrelationId<LegacyMessage>(x => x.TransactionId);

            configurator.Send<IEvent>(x =>
            {
                x.UseCorrelationId(p => p.TransactionId);
            });
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
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


    [TestFixture]
    public class TopologySetParitioning_Specs :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_handle_named_property()
        {
            var transactionId = NewId.NextGuid();

            await Bus.Publish<PartitionedMessage>(new { CorrelationId = transactionId });

            ConsumeContext<PartitionedMessage> otherContext = await _otherHandled;

            Assert.Multiple(() =>
            {
                Assert.That(otherContext.CorrelationId.HasValue, Is.True);
                Assert.That(otherContext.CorrelationId.Value, Is.EqualTo(transactionId));
            });
        }

        Task<ConsumeContext<PartitionedMessage>> _otherHandled;

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.Send<PartitionedMessage>(x =>
            {
                x.UsePartitionKeyFormatter(p => p.Message.CorrelationId.ToString("N"));
            });

            configurator.Publish<PartitionedMessage>(x =>
            {
                x.EnablePartitioning = true;
            });

            configurator.ReceiveEndpoint("partitioned-input-queue", x =>
            {
                x.EnablePartitioning = true;

                _otherHandled = Handled<PartitionedMessage>(x);
            });
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }


        public interface PartitionedMessage
        {
            Guid CorrelationId { get; }
        }
    }


    [TestFixture]
    public class TopologySetParitioningSubscription_Specs :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_handle_named_property()
        {
            var transactionId = NewId.NextGuid();

            await Bus.Publish<PartitionedMessage>(new { CorrelationId = transactionId });

            ConsumeContext<PartitionedMessage> otherContext = await _otherHandled;

            Assert.Multiple(() =>
            {
                Assert.That(otherContext.CorrelationId.HasValue, Is.True);
                Assert.That(otherContext.CorrelationId.Value, Is.EqualTo(transactionId));
            });
        }

        Task<ConsumeContext<PartitionedMessage>> _otherHandled;

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.Send<PartitionedMessage>(x =>
            {
                x.UsePartitionKeyFormatter(p => p.Message.CorrelationId.ToString("N"));
            });

            configurator.Publish<PartitionedMessage>(x =>
            {
                x.EnablePartitioning = true;
                //x.EnableExpress = true;
            });

            configurator.SubscriptionEndpoint<PartitionedMessage>("part-sub", x =>
            {
                _otherHandled = Handled<PartitionedMessage>(x);
            });
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }


        public interface PartitionedMessage
        {
            Guid CorrelationId { get; }
        }
    }
}
