namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Contexts;
    using Transports;


    public interface IClientContextSupervisor :
        ITransportSupervisor<ClientContext>
    {
    }
}
