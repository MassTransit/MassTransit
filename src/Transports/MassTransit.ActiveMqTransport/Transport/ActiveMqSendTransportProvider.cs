namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ActiveMqSendTransportProvider :
        ISendTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly ISessionContextSupervisor _modelContextSupervisor;

        public ActiveMqSendTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, ISessionContextSupervisor modelContextSupervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _modelContextSupervisor = modelContextSupervisor;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _connectionContextSupervisor.NormalizeAddress(address);
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            return _connectionContextSupervisor.CreateSendTransport(_modelContextSupervisor, address);
        }
    }
}
