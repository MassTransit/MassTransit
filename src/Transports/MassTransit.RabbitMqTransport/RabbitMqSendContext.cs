namespace MassTransit
{
    using RabbitMQ.Client;


    public interface RabbitMqSendContext :
        SendContext,
        RoutingKeySendContext
    {
        /// <summary>
        /// Specify that the published message must be delivered to a queue or it will be returned
        /// </summary>
        bool Mandatory { get; set; }

        /// <summary>
        /// The destination exchange for the message
        /// </summary>
        string Exchange { get; }

        /// <summary>
        /// True if the ack from the broker should be awaited, otherwise only the BasicPublish call is awaited
        /// </summary>
        bool AwaitAck { get; set; }

        /// <summary>
        /// The basic properties for the RabbitMQ message
        /// </summary>
        IBasicProperties BasicProperties { get; }
    }


    public interface RabbitMqSendContext<out T> :
        SendContext<T>,
        RabbitMqSendContext
        where T : class
    {
    }
}
