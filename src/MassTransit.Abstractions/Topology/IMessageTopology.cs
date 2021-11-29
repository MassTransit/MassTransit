namespace MassTransit
{
    using Configuration;


    public interface IMessageTopology<in TMessage>
        where TMessage : class
    {
        /// <summary>
        /// The entity name formatter for this message type
        /// </summary>
        IMessageEntityNameFormatter<TMessage> EntityNameFormatter { get; }

        /// <summary>
        /// The formatted entity name for this message type
        /// </summary>
        string EntityName { get; }
    }


    public interface IMessageTopology :
        IMessageTopologyConfigurationObserverConnector
    {
        /// <summary>
        /// The entity name formatter used to format message names
        /// </summary>
        IEntityNameFormatter EntityNameFormatter { get; }

        /// <summary>
        /// Returns the message topology for the specified message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IMessageTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
