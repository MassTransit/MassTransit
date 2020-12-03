namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Transports;


    public interface IClientContextSupervisor :
        ITransportSupervisor<ClientContext>
    {
    }
}
