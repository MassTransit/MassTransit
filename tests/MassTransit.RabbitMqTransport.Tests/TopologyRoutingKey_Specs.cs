namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class TopologyRoutingKey_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_handle_base_event_class()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<IRoutedEvent>(new { TransactionId = transactionId });

            ConsumeContext<IRoutedEvent> context = await _handled;

            Assert.IsTrue(context.CorrelationId.HasValue);
            Assert.That(context.CorrelationId.Value, Is.EqualTo(transactionId));

            Assert.IsTrue(context.TryGetPayload<RabbitMqBasicConsumeContext>(out var basicConsumeContext));
            Assert.That(basicConsumeContext.RoutingKey, Is.EqualTo(transactionId.ToString()));
        }

        Task<ConsumeContext<IRoutedEvent>> _handled;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Send<IRoutedEvent>(x => x.UseRoutingKeyFormatter(context => context.Message.TransactionId.ToString()));

            configurator.Send<IEvent>(x =>
            {
                x.UseCorrelationId(p => p.TransactionId);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<IRoutedEvent>(configurator);
        }


        public interface IEvent
        {
            Guid TransactionId { get; }
        }


        public interface IRoutedEvent :
            IEvent
        {
        }
    }


    [TestFixture]
    public class Setting_the_routing_key_topology_on_the_subclass :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_include_the_routing_key()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<IRoutedEvent>(new { TransactionId = transactionId });

            ConsumeContext<IRoutedEvent> context = await _handled;

            Assert.That(context.RoutingKey(), Is.EqualTo(transactionId.ToString()));
        }

        Task<ConsumeContext<IRoutedEvent>> _handled;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Send<IRoutedEvent>(x => x.UseRoutingKeyFormatter(context => context.Message.TransactionId.ToString()));
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<IRoutedEvent>(configurator);
        }


        public interface IEvent
        {
            Guid TransactionId { get; }
        }


        public interface IRoutedEvent :
            IEvent
        {
        }
    }


    [TestFixture]
    public class Setting_the_routing_key_topology_on_the_interface :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_include_the_routing_key()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<IRoutedEvent>(new { TransactionId = transactionId });

            ConsumeContext<IRoutedEvent> context = await _handled;

            Assert.That(context.RoutingKey(), Is.EqualTo(transactionId.ToString()));
        }

        Task<ConsumeContext<IRoutedEvent>> _handled;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Send<IEvent>(x => x.UseRoutingKeyFormatter(context => context.Message.TransactionId.ToString()));
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<IRoutedEvent>(configurator);
        }


        public interface IEvent
        {
            Guid TransactionId { get; }
        }


        public interface IRoutedEvent :
            IEvent
        {
        }
    }
}
