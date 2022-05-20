#nullable enable
namespace MassTransit.Transports.Fabric
{
    using System;
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
        /// Optional routing key, which is used by direct/topic exchanges
        /// </summary>
        string? RoutingKey { get; }

        /// <summary>
        /// Optional enqueue time, which can be used to delay messages
        /// </summary>
        DateTime? EnqueueTime { get; }

        /// <summary>
        /// If specified, targets a specific receiver in the message fabric
        /// </summary>
        long? ReceiverId { get; }

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
