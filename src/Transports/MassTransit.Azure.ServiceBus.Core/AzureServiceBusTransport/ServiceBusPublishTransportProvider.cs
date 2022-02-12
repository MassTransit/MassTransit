#nullable enable
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ServiceBusPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly ReceiveEndpointContext _context;

        public ServiceBusPublishTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, ReceiveEndpointContext context)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _context = context;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri? publishAddress)
            where T : class
        {
            return _connectionContextSupervisor.CreatePublishTransport<T>(_context, publishAddress);
        }
    }
}
