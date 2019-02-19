namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Returned when a receive endpoint is added to a host
    /// </summary>
    public interface HostReceiveEndpointHandle
    {
        IReceiveEndpoint ReceiveEndpoint { get; }

        /// <summary>
        /// A task which can be awaited to know when the receive endpoint is ready
        /// </summary>
        Task<ReceiveEndpointReady> Ready { get; }

        /// <summary>
        /// Stop the receive endpoint.
        /// </summary>
        /// <param name="cancellationToken">Cancel the stop operation in progress</param>
        /// <returns>An awaitable task that is completed once everything is stopped</returns>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
