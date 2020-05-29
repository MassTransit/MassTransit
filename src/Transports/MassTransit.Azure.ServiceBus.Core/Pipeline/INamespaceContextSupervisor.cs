namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using GreenPipes.Agents;


    public interface INamespaceContextSupervisor :
        ISupervisor<NamespaceContext>
    {
    }
}
