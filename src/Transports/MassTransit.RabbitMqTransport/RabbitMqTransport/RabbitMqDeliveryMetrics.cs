namespace MassTransit.RabbitMqTransport
{
    using Transports;


    public interface RabbitMqDeliveryMetrics :
        DeliveryMetrics
    {
        /// <summary>
        /// The consumer tag that was assigned to the consumer by the broker
        /// </summary>
        string ConsumerTag { get; }
    }
}
