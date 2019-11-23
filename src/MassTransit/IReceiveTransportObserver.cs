namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Events;


    /// <summary>
    /// Used to observe the events signaled by a receive endpoint
    /// </summary>
    public interface IReceiveTransportObserver
    {
        /// <summary>
        /// Called when the receive endpoint is ready to receive messages
        /// </summary>
        /// <param name="ready"></param>
        /// <returns></returns>
        Task Ready(ReceiveTransportReady ready);

        /// <summary>
        /// Called when the receive endpoint has completed
        /// </summary>
        /// <param name="completed"></param>
        /// <returns></returns>
        Task Completed(ReceiveTransportCompleted completed);

        /// <summary>
        /// Called when the receive endpoint faults
        /// </summary>
        /// <param name="faulted"></param>
        /// <returns></returns>
        Task Faulted(ReceiveTransportFaulted faulted);
    }


    public static class ReceiveTransportObserverExtensions
    {
        public static Task NotifyReady(this IReceiveTransportObserver observer, Uri inputAddress)
        {
            return observer.Ready(new ReceiveTransportReadyEvent(inputAddress));
        }
    }
}
