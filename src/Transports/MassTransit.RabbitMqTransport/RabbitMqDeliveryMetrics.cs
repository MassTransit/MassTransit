namespace MassTransit.RabbitMqTransport
{
    using Transports.Metrics;


    public interface RabbitMqDeliveryMetrics :
        DeliveryMetrics
    {
        /// <summary>
        /// The consumer tag that was assigned to the consumer by the broker
        /// </summary>
        string ConsumerTag { get; }
    }
}
