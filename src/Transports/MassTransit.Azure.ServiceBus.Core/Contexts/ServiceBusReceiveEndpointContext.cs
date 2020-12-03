namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using Context;
    using Pipeline;
    using Topology;


    public interface ServiceBusReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }
    }
}
