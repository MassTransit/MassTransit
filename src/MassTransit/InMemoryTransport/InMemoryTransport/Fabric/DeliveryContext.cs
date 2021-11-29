namespace MassTransit.InMemoryTransport.Fabric
{
    using System.Threading;


    public interface DeliveryContext<T>
        where T : class
    {
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// The package being delivered
        /// </summary>
        T Message { get; }

        /// <summary>
        /// Should this delivery occur, or has is already been delivered
        /// </summary>
        /// <param name="sink"></param>
        /// <returns></returns>
        bool WasAlreadyDelivered(IMessageSink<T> sink);

        /// <summary>
        /// Marks the sink as delivered for this dispatch
        /// </summary>
        /// <param name="sink"></param>
        void Delivered(IMessageSink<T> sink);
    }
}
