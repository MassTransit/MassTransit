namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class AmazonSqsSendTransportProvider :
        ISendTransportProvider
    {
        readonly IClientContextSupervisor _clientContextSupervisor;
        readonly IConnectionContextSupervisor _connectionContextSupervisor;

        public AmazonSqsSendTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, IClientContextSupervisor clientContextSupervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _clientContextSupervisor = clientContextSupervisor;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _connectionContextSupervisor.NormalizeAddress(address);
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            return _connectionContextSupervisor.CreateSendTransport(_clientContextSupervisor, address);
        }
    }
}
