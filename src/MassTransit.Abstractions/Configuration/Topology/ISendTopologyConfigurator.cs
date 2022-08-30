namespace MassTransit
{
    using Configuration;


    public interface ISendTopologyConfigurator :
        ISendTopology,
        ISpecification
    {
        /// <summary>
        /// Specify a dead letter queue name formatter, which is used to format the name for a dead letter queue.
        /// Defaults to (queue name)_skipped.
        /// </summary>
        public new IDeadLetterQueueNameFormatter DeadLetterQueueNameFormatter { get; set; }

        /// <summary>
        /// Specify an error queue name formatter, which is used to format the name for an error queue.
        /// Defaults to (queue name)_error.
        /// </summary>
        public new IErrorQueueNameFormatter ErrorQueueNameFormatter { get; set; }

        /// <summary>
        /// Adds a convention to the topology, which will be applied to every message type
        /// requested, to determine if a convention for the message type is available.
        /// </summary>
        /// <param name="convention">The send topology convention</param>
        bool TryAddConvention(ISendTopologyConvention convention);

        /// <summary>
        /// Add a send topology for a specific message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="topology">The topology</param>
        void AddMessageSendTopology<T>(IMessageSendTopology<T> topology)
            where T : class;
    }
}
