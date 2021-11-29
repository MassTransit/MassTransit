namespace MassTransit.Transports
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface HostHandle
    {
        /// <summary>
        /// A task which can be awaited to know when the host is ready
        /// </summary>
        Task<HostReady> Ready { get; }

        /// <summary>
        /// Close the Host, shutting it down for good.
        /// </summary>
        /// <returns></returns>
        Task Stop(CancellationToken cancellationToken = default);
    }
}
