namespace MassTransit
{
    using System;
    using Configuration;


    /// <summary>
    /// Configures the Publishing of a message type, allowing filters to be applied
    /// on Publish.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessagePublishTopologyConfigurator<TMessage> :
        IMessagePublishTopologyConfigurator,
        IMessagePublishTopology<TMessage>
        where TMessage : class
    {
        void Add(IMessagePublishTopology<TMessage> publishTopology);

        /// <summary>
        /// Adds a delegated configuration to the Publish topology, which is called before any topologies
        /// in this configuration.
        /// </summary>
        /// <param name="configuration"></param>
        void AddDelegate(IMessagePublishTopology<TMessage> configuration);

        /// <summary>
        /// Adds a convention to the message Publish topology configuration, which can be modified
        /// </summary>
        /// <param name="convention"></param>
        bool TryAddConvention(IMessagePublishTopologyConvention<TMessage> convention);

        /// <summary>
        /// Returns the first convention that matches the interface type specified, to allow it to be customized
        /// and or replaced.
        /// </summary>
        /// <typeparam name="TConvention"></typeparam>
        /// <param name="add">Called if the convention does not already exist</param>
        /// <param name="update">Called if the convention already exists</param>
        /// <returns></returns>
        void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
            where TConvention : class, IMessagePublishTopologyConvention<TMessage>;
    }


    public interface IMessagePublishTopologyConfigurator :
        IMessagePublishTopology,
        ISpecification
    {
        /// <summary>
        /// Exclude the message type from being created as a topic/exchange.
        /// </summary>
        bool Exclude { set; }
    }
}
