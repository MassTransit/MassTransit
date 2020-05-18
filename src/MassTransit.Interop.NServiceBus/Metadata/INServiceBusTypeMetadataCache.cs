namespace MassTransit.Interop.NServiceBus.Metadata
{
    public interface INServiceBusTypeMetadataCache<out T>
    {
        /// <summary>
        /// The names of all the message types supported by the message type
        /// </summary>
        string[] MessageTypeNames { get; }
    }
}