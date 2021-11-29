namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A handle to an active endpoint
    /// </summary>
    public interface ReceiveEndpointHandle
    {
        /// <summary>
        /// A task which can be awaited to know when the receive endpoint is ready
        /// </summary>
        Task<ReceiveEndpointReady> Ready { get; }

        /// <summary>
        /// Stop the endpoint, releasing any resources associated with the endpoint
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Stop(CancellationToken cancellationToken = default);
    }
}
