namespace MassTransit.Topology
{
    using System;
    using System.Linq.Expressions;


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

        /// <summary>
        /// Specify the property which should be used for the message CorrelationId
        /// </summary>
        /// <param name="propertyExpression"></param>
        void CorrelateBy(Expression<Func<TMessage, Guid>> propertyExpression);

        IMessagePropertyTopologyConfigurator<TMessage, T> GetProperty<T>(Expression<Func<TMessage, T>> propertyExpression);
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
