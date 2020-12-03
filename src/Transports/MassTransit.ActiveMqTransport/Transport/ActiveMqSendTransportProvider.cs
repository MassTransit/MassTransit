namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ActiveMqSendTransportProvider :
        ISendTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly ISessionContextSupervisor _supervisor;

        public ActiveMqSendTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, ISessionContextSupervisor supervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _supervisor = supervisor;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _connectionContextSupervisor.NormalizeAddress(address);
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            return _connectionContextSupervisor.CreateSendTransport(_supervisor, address);
        }
    }
}
