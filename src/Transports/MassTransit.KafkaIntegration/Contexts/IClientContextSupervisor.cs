namespace MassTransit.KafkaIntegration.Contexts
{
    using Transports;


    public interface IClientContextSupervisor :
        ITransportSupervisor<ClientContext>
    {
    }
}
