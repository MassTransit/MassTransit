namespace MassTransit.Transports.InMemory.Fabric
{
    public interface DeliveryContext<T>
        where T : class
    {
        /// <summary>
        /// Should this delivery occur, or has is already been delievered
        /// </summary>
        /// <param name="sink"></param>
        /// <returns></returns>
        bool WasAlreadyDelivered(IMessageSink<T> sink);

        /// <summary>
        /// Marks the sink as delivered for this dispatch
        /// </summary>
        /// <param name="sink"></param>
        void Delivered(IMessageSink<T> sink);

        /// <summary>
        /// The package being delivered
        /// </summary>
        T Package { get; }
    }
}