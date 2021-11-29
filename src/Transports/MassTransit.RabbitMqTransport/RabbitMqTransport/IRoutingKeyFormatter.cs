namespace MassTransit.RabbitMqTransport
{
    public interface IRoutingKeyFormatter
    {
        /// <summary>
        /// Format the routing key for the send context, so that it can be passed to RabbitMQ
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message send context</param>
        /// <returns>The routing key to specify in the transport</returns>
        string FormatRoutingKey<T>(RabbitMqSendContext<T> context)
            where T : class;
    }
}
