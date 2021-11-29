namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class RabbitMqSendTransportProvider :
        ISendTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly IModelContextSupervisor _modelContextSupervisor;
        readonly RabbitMqReceiveEndpointContext _receiveEndpointContext;

        public RabbitMqSendTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, RabbitMqReceiveEndpointContext receiveEndpointContext)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _modelContextSupervisor = receiveEndpointContext.ModelContextSupervisor;
            _receiveEndpointContext = receiveEndpointContext;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _connectionContextSupervisor.NormalizeAddress(address);
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            return _connectionContextSupervisor.CreateSendTransport(_receiveEndpointContext,_modelContextSupervisor, address);
        }
    }
}
