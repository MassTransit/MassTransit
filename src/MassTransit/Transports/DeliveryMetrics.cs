namespace MassTransit.Transports
{
    public interface DeliveryMetrics
    {
        /// <summary>
        /// The number of messages consumed by the consumer
        /// </summary>
        long DeliveryCount { get; }

        /// <summary>
        /// The highest concurrent message count that was received by the consumer
        /// </summary>
        int ConcurrentDeliveryCount { get; }
    }
}
