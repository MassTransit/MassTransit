namespace MassTransit
{
    public interface IOutboxConfigurator
    {
        /// <summary>
        /// Set to true if messages can be delivered to the broker concurrently. Concurrent delivery is faster, but does not match the order of the
        /// original publish/respond/send calls. Defaults to false to match existing behavior.
        /// </summary>
        bool ConcurrentMessageDelivery { set; }
    }
}
