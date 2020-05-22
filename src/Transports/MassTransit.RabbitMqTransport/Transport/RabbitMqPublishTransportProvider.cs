namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Integration;
    using Transports;


    public class RabbitMqPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly IModelContextSupervisor _supervisor;

        public RabbitMqPublishTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, IModelContextSupervisor supervisor)
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
