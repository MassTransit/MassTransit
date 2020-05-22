namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Agents;
    using Transports;


    public interface IConnectionContextSupervisor :
        ISupervisor<ConnectionContext>
    {
        Uri NormalizeAddress(Uri address);

        Task<ISendTransport> CreateSendTransport(IModelContextSupervisor modelContextSupervisor, Uri address);

        Task<ISendTransport> CreatePublishTransport<T>(IModelContextSupervisor modelContextSupervisor)
            where T : class;
    }
}
