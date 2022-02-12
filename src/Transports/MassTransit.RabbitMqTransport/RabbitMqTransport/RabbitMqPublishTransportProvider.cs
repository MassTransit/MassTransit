#nullable enable
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class RabbitMqPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly IModelContextSupervisor _supervisor;
        readonly RabbitMqReceiveEndpointContext _receiveEndpointContext;

        public RabbitMqPublishTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, RabbitMqReceiveEndpointContext receiveEndpointContext)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _supervisor = receiveEndpointContext.ModelContextSupervisor;
            _receiveEndpointContext = receiveEndpointContext;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri? publishAddress)
            where T : class
        {
            return _connectionContextSupervisor.CreatePublishTransport<T>(_receiveEndpointContext, _supervisor);
        }
    }
}
