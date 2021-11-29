namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ServiceBusSendTransportProvider :
        ISendTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly ReceiveEndpointContext _context;

        public ServiceBusSendTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, ReceiveEndpointContext context)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _context = context;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _connectionContextSupervisor.NormalizeAddress(address);
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            return _connectionContextSupervisor.CreateSendTransport(_context, address);
        }
    }
}
