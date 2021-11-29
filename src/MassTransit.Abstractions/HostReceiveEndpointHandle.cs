namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Returned when a receive endpoint is connected
    /// </summary>
    public interface HostReceiveEndpointHandle
    {
        IReceiveEndpoint ReceiveEndpoint { get; }

        /// <summary>
        /// Completed when the endpoint has successfully started and is ready to consume messages.
        /// </summary>
        Task<ReceiveEndpointReady> Ready { get; }

        /// <summary>
        /// Stop the receive endpoint and remove it from the host. Once removed, the endpoint
        /// cannot be restarted using the <see cref="ReceiveEndpoint"/> property directly.
        /// </summary>
        /// <param name="cancellationToken">Cancel the stop operation in progress</param>
        /// <returns>Completed once the receive endpoint has stopped and been removed from the host</returns>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
