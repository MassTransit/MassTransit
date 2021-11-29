namespace MassTransit.Configuration
{
    public interface IMessageTopologyConfigurator<TMessage> :
        IMessageTypeTopologyConfigurator,
        IMessageTopology<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Sets the entity name formatter used for this message type
        /// </summary>
        /// <param name="entityNameFormatter"></param>
        void SetEntityNameFormatter(IMessageEntityNameFormatter<TMessage> entityNameFormatter);

        /// <summary>
        /// Sets the entity name for this message type
        /// </summary>
        /// <param name="entityName">The entity name</param>
        void SetEntityName(string entityName);
    }


    public interface IMessageTopologyConfigurator :
        IMessageTopology
    {
        /// <summary>
        /// Replace the default entity name formatter
        /// </summary>
        void SetEntityNameFormatter(IEntityNameFormatter entityNameFormatter);

        new IMessageTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
