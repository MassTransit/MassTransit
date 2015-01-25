namespace MassTransit.Transports
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A Bus Host is a transport-neutral reference to a host
    /// </summary>
    public interface IBusHost
    {
        /// <summary>
        /// Close the bus, freeing any resources that may be in use
        /// </summary>
        /// <returns></returns>
        Task Close(CancellationToken cancellationToken = default(CancellationToken));
    }
}