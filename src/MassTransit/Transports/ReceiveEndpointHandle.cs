namespace MassTransit.Transports
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A handle to an active endpoint
    /// </summary>
    public interface ReceiveEndpointHandle
    {
        /// <summary>
        /// Stop the endpoint, releasing any resources associated with the endpoint
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Stop(CancellationToken cancellationToken = default);
    }
}
