namespace MassTransit.Transports
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes.Agents;


    public interface IBusHostControl :
        ISupervisor,
        IHost
    {
        /// <summary>
        /// Starts the Host, which begins the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<HostHandle> Start(CancellationToken cancellationToken);

        void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint);
    }
}
