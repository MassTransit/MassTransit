namespace MassTransit.GrpcTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fabric;
    using Transports.Fabric;


    static class GrpcTransportHelperExtensions
    {
        public static Contracts.ExchangeType ToGrpcExchangeType(this Transports.Fabric.ExchangeType exchangeType)
        {
            return exchangeType switch
            {
                ExchangeType.Direct => Contracts.ExchangeType.Direct,
                ExchangeType.Topic => Contracts.ExchangeType.Topic,
                ExchangeType.FanOut => Contracts.ExchangeType.FanOut,
                _ => throw new ArgumentException(nameof(exchangeType))
            };
        }

        public static Transports.Fabric.ExchangeType ToExchangeType(this Contracts.ExchangeType exchangeType)
        {
            return exchangeType switch
            {
                Contracts.ExchangeType.Direct => ExchangeType.Direct,
                Contracts.ExchangeType.Topic => ExchangeType.Topic,
                Contracts.ExchangeType.FanOut => ExchangeType.FanOut,
                _ => throw new ArgumentException(nameof(exchangeType))
            };
        }

        public static Task Send(this IMessageExchange<GrpcTransportMessage> exchange, GrpcTransportMessage message, CancellationToken cancellationToken =
            default)
        {
            var deliveryContext = new GrpcDeliveryContext(message, cancellationToken);

            return exchange.Deliver(deliveryContext);
        }

        public static Task Send(this IMessageQueue<NodeContext, GrpcTransportMessage> queue, GrpcTransportMessage message,
            CancellationToken cancellationToken = default)
        {
            var deliveryContext = new GrpcDeliveryContext(message, cancellationToken);

            return queue.Deliver(deliveryContext);
        }
    }
}
