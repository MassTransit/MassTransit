namespace MassTransit
{
    using RabbitMQ.Client;


    /// <summary>
    /// Contains the context of the BasicConsume call received by the BasicConsumer
    /// bound to the inbound RabbitMQ channel
    /// </summary>
    public interface RabbitMqBasicConsumeContext :
        RoutingKeyConsumeContext
    {
        /// <summary>
        /// The exchange to which to the message was sent
        /// </summary>
        string Exchange { get; }

        /// <summary>
        /// The consumer tag of the receiving consumer
        /// </summary>
        string ConsumerTag { get; }

        /// <summary>
        /// The delivery tag of the message to the consumer
        /// </summary>
        ulong DeliveryTag { get; }

        /// <summary>
        /// The basic properties of the message
        /// </summary>
        IReadOnlyBasicProperties Properties { get; }
    }
}
