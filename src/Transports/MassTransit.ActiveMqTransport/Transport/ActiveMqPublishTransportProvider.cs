namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ActiveMqPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly ISessionContextSupervisor _supervisor;

        public ActiveMqPublishTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, ISessionContextSupervisor supervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _supervisor = supervisor;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            return _connectionContextSupervisor.CreatePublishTransport<T>(_supervisor);
        }
    }
}
