namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class AmazonSqsPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly IClientContextSupervisor _clientContextSupervisor;

        public AmazonSqsPublishTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, IClientContextSupervisor clientContextSupervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _clientContextSupervisor = clientContextSupervisor;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            return _connectionContextSupervisor.CreatePublishTransport<T>(_clientContextSupervisor);
        }
    }
}
