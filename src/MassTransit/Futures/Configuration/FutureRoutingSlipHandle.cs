namespace MassTransit.Futures
{
    using Automatonymous;
    using Courier.Contracts;


    public interface FutureRoutingSlipHandle
    {
        /// <summary>
        /// The fault state machine event
        /// </summary>
        Event<RoutingSlipFaulted> Faulted { get; }

        /// <summary>
        /// The response state machine event
        /// </summary>
        Event<RoutingSlipCompleted> Completed { get; }
    }
}
