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

            Assert.Multiple(() =>
            {
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(transactionId));
            });

            var settings = GetHostSettings();

            var connectionFactory = settings.GetConnectionFactory();

            await using var connection = settings.EndpointResolver != null
                ? await connectionFactory.CreateConnectionAsync(settings.EndpointResolver, settings.Host)
                : await connectionFactory.CreateConnectionAsync();

            await using var channel = await connection.CreateChannelAsync();

            var eventExchangeName = RabbitMqBusFactory.CreateMessageTopology().EntityNameFormatter.FormatEntityName<IEvent>();

            Assert.That(async () => await channel.ExchangeDeclarePassiveAsync(eventExchangeName), Throws.TypeOf<OperationInterruptedException>().With
                .Property("ShutdownReason").Property("ReplyCode").EqualTo(404));

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

        protected override async Task OnCleanupVirtualHost(IChannel channel)
        {
            await base.OnCleanupVirtualHost(channel);

            var eventExchangeName = RabbitMqBusFactory.CreateMessageTopology().EntityNameFormatter.FormatEntityName<IEvent>();
            await channel.ExchangeDeleteAsync(eventExchangeName);

            var routedEventExchangeName = RabbitMqBusFactory.CreateMessageTopology().EntityNameFormatter.FormatEntityName<TransactionEvent>();
            await channel.ExchangeDeleteAsync(routedEventExchangeName);
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
