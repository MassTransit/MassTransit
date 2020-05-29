namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using GreenPipes.Agents;


    public interface IClientContextSupervisor :
        ISupervisor<ClientContext>
    {
    }
}
