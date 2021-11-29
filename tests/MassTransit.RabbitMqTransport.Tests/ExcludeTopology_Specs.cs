namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    using TopologyTests;


    [TestFixture]
    public class Excluding_a_type_from_topology :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_not_create_the_exchange()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<TransactionEvent>(new { TransactionId = transactionId });

            ConsumeContext<TransactionEvent> context = await _handled;

            Assert.IsTrue(context.CorrelationId.HasValue);
            Assert.That(context.CorrelationId.Value, Is.EqualTo(transactionId));

            var settings = GetHostSettings();

            var connectionFactory = settings.GetConnectionFactory();

            using var connection = settings.EndpointResolver != null
                ? connectionFactory.CreateConnection(settings.EndpointResolver, settings.Host)
                : connectionFactory.CreateConnection();

            using var model = connection.CreateModel();

            var eventExchangeName = RabbitMqBusFactory.MessageTopology.EntityNameFormatter.FormatEntityName<IEvent>();

            var exception = Assert.Throws<OperationInterruptedException>(() => model.ExchangeDeclarePassive(eventExchangeName));

            Assert.That(exception.ShutdownReason.ReplyCode, Is.EqualTo(404));
        }

        Task<ConsumeContext<TransactionEvent>> _handled;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Send<IEvent>(x =>
            {
                x.UseCorrelationId(p => p.TransactionId);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<TransactionEvent>(configurator);
        }

        protected override void OnCleanupVirtualHost(IModel model)
        {
            base.OnCleanupVirtualHost(model);

            var eventExchangeName = RabbitMqBusFactory.MessageTopology.EntityNameFormatter.FormatEntityName<IEvent>();
            model.ExchangeDelete(eventExchangeName);

            var routedEventExchangeName = RabbitMqBusFactory.MessageTopology.EntityNameFormatter.FormatEntityName<TransactionEvent>();
            model.ExchangeDelete(routedEventExchangeName);
        }
    }


    namespace TopologyTests
    {
        using System;


        [ExcludeFromTopology]
        public interface IEvent
        {
            Guid TransactionId { get; }
        }


        public interface TransactionEvent :
            IEvent
        {
        }
    }
}
