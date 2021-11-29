namespace MassTransit
{
    using System;
    using Configuration;


    /// <summary>
    /// Configures the Consuming of a message type, allowing filters to be applied
    /// on Consume.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageConsumeTopologyConfigurator<TMessage> :
        IMessageConsumeTopologyConfigurator,
        IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Specify whether the broker topology should be configured for this message type
        /// (defaults to true)
        /// </summary>
        bool ConfigureConsumeTopology { get; set; }

        void Add(IMessageConsumeTopology<TMessage> consumeTopology);

        /// <summary>
        /// Adds a delegated configuration to the Consume topology, which is called before any topologies
        /// in this configuration.
        /// </summary>
        /// <param name="configuration"></param>
        void AddDelegate(IMessageConsumeTopology<TMessage> configuration);

        /// <summary>
        /// Adds a convention to the message Consume topology configuration, which can be modified
        /// </summary>
        /// <param name="convention"></param>
        bool TryAddConvention(IMessageConsumeTopologyConvention<TMessage> convention);

        /// <summary>
        /// Update a convention if available, otherwise, throw an exception
        /// </summary>
        /// <typeparam name="TConvention"></typeparam>
        /// <param name="update">Called if the convention already exists</param>
        /// <returns></returns>
        void UpdateConvention<TConvention>(Func<TConvention, TConvention> update)
            where TConvention : class, IMessageConsumeTopologyConvention<TMessage>;

        /// <summary>
        /// Returns the first convention that matches the interface type specified, to allow it to be customized
        /// and or replaced.
        /// </summary>
        /// <typeparam name="TConvention"></typeparam>
        /// <param name="add">Called if the convention does not already exist</param>
        /// <param name="update">Called if the convention already exists</param>
        /// <returns></returns>
        void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
            where TConvention : class, IMessageConsumeTopologyConvention<TMessage>;
    }


    public interface IMessageConsumeTopologyConfigurator :
        ISpecification
    {
        bool TryAddConvention(IConsumeTopologyConvention convention);
    }
}
