namespace MassTransit.RabbitMqTransport
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using RabbitMQ.Client;


    /// <summary>
    /// With a connection, and a channel from RabbitMQ, this context is passed forward to allow
    /// the channel to be configured and connected
    /// </summary>
    public interface ChannelContext :
        PipeContext
    {
        /// <summary>
        /// The channel
        /// </summary>
        IChannel Channel { get; }

        /// <summary>
        /// The connection context on which the channel was created
        /// </summary>
        ConnectionContext ConnectionContext { get; }

        /// <summary>
        /// Publish a message to the broker, asynchronously
        /// </summary>
        /// <param name="exchange">The destination exchange</param>
        /// <param name="routingKey">The exchange routing key</param>
        /// <param name="mandatory">true if the message must be delivered</param>
        /// <param name="basicProperties">The message properties</param>
        /// <param name="body">The message body</param>
        /// <param name="awaitAck"></param>
        /// <returns>
        /// An awaitable Task that is completed when the message is acknowledged by the broker
        /// </returns>
        Task BasicPublishAsync(string exchange, string routingKey, bool mandatory, BasicProperties basicProperties, byte[] body, bool awaitAck);

        Task ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments);
        Task ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments);
        Task ExchangeDeclarePassive(string exchange);

        Task QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments);
        Task<QueueDeclareOk> QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments);
        Task<QueueDeclareOk> QueueDeclarePassive(string queue);

        Task<uint> QueuePurge(string queue);

        Task BasicQos(uint prefetchSize, ushort prefetchCount, bool global);

        ValueTask BasicAck(ulong deliveryTag, bool multiple);

        Task BasicNack(ulong deliveryTag, bool multiple, bool requeue);

        Task<string> BasicConsume(string queue, bool noAck, bool exclusive, IDictionary<string, object> arguments, IAsyncBasicConsumer consumer,
            string consumerTag, CancellationToken cancellationToken);

        Task BasicCancel(string consumerTag);
    }
}
