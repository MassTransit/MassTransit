namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Contexts;
    using GreenPipes.Agents;


    public interface IConnectionContextSupervisor :
        ISupervisor<ConnectionContext>
    {
    }
}
