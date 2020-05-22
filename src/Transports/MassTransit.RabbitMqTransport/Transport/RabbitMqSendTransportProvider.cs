namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Integration;
    using Transports;


    public class RabbitMqSendTransportProvider :
        ISendTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly IModelContextSupervisor _modelContextSupervisor;

        public RabbitMqSendTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, IModelContextSupervisor modelContextSupervisor)
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
