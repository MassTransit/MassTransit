namespace MassTransit.Metadata
{
    public interface IImplementedMessageTypeCache<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Invokes the interface for each implemented type of the message
        /// </summary>
        /// <param name="implementedMessageType"></param>
        /// <param name="includeActualType"></param>
        void EnumerateImplementedTypes(IImplementedMessageType implementedMessageType, bool includeActualType);
    }
}
