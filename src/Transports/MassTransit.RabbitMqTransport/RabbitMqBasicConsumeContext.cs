namespace MassTransit
{
    using System;
    using RabbitMQ.Client;


    /// <summary>
    /// Contains the context of the BasicConsume call received by the BasicConsumer
    /// bound to the inbound RabbitMQ model
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
        IBasicProperties Properties { get; }

        /// <summary>
        /// The message body, since it's a byte array on RabbitMQ
        /// </summary>
        [Obsolete("This is a fail, we need to use the Body of receive context")]
        byte[] Body { get; }
    }
}
