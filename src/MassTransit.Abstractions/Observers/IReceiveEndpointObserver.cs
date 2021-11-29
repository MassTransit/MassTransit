namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Used to observe the events signaled by a receive endpoint
    /// </summary>
    public interface IReceiveEndpointObserver
    {
        /// <summary>
        /// Called when the receive endpoint is ready to receive messages
        /// </summary>
        /// <param name="ready"></param>
        /// <returns></returns>
        Task Ready(ReceiveEndpointReady ready);

        /// <summary>
        /// Called when the receive endpoint is being stopped, prior to actually stopping
        /// </summary>
        /// <param name="stopping"></param>
        /// <returns></returns>
        Task Stopping(ReceiveEndpointStopping stopping);

        /// <summary>
        /// Called when the receive endpoint has completed
        /// </summary>
        /// <param name="completed"></param>
        /// <returns></returns>
        Task Completed(ReceiveEndpointCompleted completed);

        /// <summary>
        /// Called when the receive endpoint faults
        /// </summary>
        /// <param name="faulted"></param>
        /// <returns></returns>
        Task Faulted(ReceiveEndpointFaulted faulted);
    }
}
