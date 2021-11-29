namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Kafka messages are a combination of headers, a key, and a value.
    /// </summary>
    /// <typeparam name="TKey">The Kafka topic key type</typeparam>
    /// <typeparam name="TValue">The Kafka topic value type</typeparam>
    public interface ITopicProducer<TKey, TValue> :
        ISendObserverConnector
        where TValue : class
    {
        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="key">The key, matching the topic type</param>
        /// <param name="value">The message value</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="key">The key, matching the topic type</param>
        /// <param name="value">The message value</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="key">The key, matching the topic type</param>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TKey key, object values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="key">The key, matching the topic type</param>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TKey key, object values, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default);
    }


    /// <summary>
    /// Kafka messages are a combination of headers, a Null key type, and a value.
    /// </summary>
    /// <typeparam name="TValue">The Kafka topic value type</typeparam>
    public interface ITopicProducer<TValue> :
        ISendObserverConnector
        where TValue : class
    {
        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="message">The message value</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TValue message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="message">The message value</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TValue message, IPipe<KafkaSendContext<TValue>> pipe, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="cancellationToken"></param>
        Task Produce(object values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce(object values, IPipe<KafkaSendContext<TValue>> pipe, CancellationToken cancellationToken = default);
    }
}
