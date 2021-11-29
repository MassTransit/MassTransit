namespace MassTransit
{
    /// <summary>
    /// Used to build entity names for the publish topology
    /// <typeparam name="TMessage"></typeparam>
    /// </summary>
    public interface IMessageEntityNameFormatter<in TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Formats the entity name for the given message
        /// </summary>
        /// <returns></returns>
        string FormatEntityName();
    }
}
