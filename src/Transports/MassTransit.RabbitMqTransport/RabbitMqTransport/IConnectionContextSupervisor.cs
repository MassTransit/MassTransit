namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>
    {
        Uri NormalizeAddress(Uri address);

        Task<ISendTransport> CreateSendTransport(RabbitMqReceiveEndpointContext receiveEndpointContext, IModelContextSupervisor modelContextSupervisor,
            Uri address);

        Task<ISendTransport> CreatePublishTransport<T>(RabbitMqReceiveEndpointContext receiveEndpointContext, IModelContextSupervisor modelContextSupervisor)
            where T : class;
    }
}
