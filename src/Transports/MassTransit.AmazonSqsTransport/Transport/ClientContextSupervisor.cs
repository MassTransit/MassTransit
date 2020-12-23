namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;

        public ClientContextSupervisor(IConnectionContextSupervisor supervisor)
            : base(new ClientContextFactory(supervisor))
        {
            _connectionContextSupervisor = supervisor;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _connectionContextSupervisor.NormalizeAddress(address);
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            return _connectionContextSupervisor.CreateSendTransport(this, address);
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            return _connectionContextSupervisor.CreatePublishTransport<T>(this);
        }
    }
}
