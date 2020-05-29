namespace MassTransit
{
    /// <summary>
    /// Defines an endpoint in a transport-independent way
    /// </summary>
    public interface IEndpointDefinition
    {
        /// <summary>
        /// True if the endpoint is temporary, and should be removed when the bus/endpoint is stopped. Temporary queues
        /// should be configured as auto-delete, non-durable, express, whatever creates the least impact and fastest performance.
        /// </summary>
        bool IsTemporary { get; }

        /// <summary>
        /// The number of messages to fetch in advance from the broker, if applicable. This should <b>only</b> be set when
        /// necessary, use the <see cref="ConcurrentMessageLimit" /> initially.
        /// </summary>
        int? PrefetchCount { get; }

        /// <summary>
        /// The maximum number of concurrent messages which can be delivered at any one time. This should be set by an
        /// endpoint before modifying the prefetch count. If this is specified, and <see cref="PrefetchCount" /> is left default,
        /// it will calculate an effective prefetch count automatically when supported.
        /// </summary>
        int? ConcurrentMessageLimit { get; }

        /// <summary>
        /// If true, configure the broker topology, which may include binding exchanges, subscribing to topics, etc.
        /// </summary>
        bool ConfigureConsumeTopology { get; }

        /// <summary>
        /// Return the endpoint name for the consumer, using the specified formatter if necessary.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        string GetEndpointName(IEndpointNameFormatter formatter);

        /// <summary>
        /// Configure the endpoint, as provided by the transport-specific receive endpoint configurator
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T"></typeparam>
        void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator;
    }


    public interface IEndpointDefinition<T> :
        IEndpointDefinition
        where T : class
    {
    }
}
