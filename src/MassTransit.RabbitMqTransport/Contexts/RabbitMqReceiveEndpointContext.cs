namespace MassTransit.RabbitMqTransport.Contexts
{
    using Context;
    using Integration;
    using Topology.Builders;


    public interface RabbitMqReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        bool ExclusiveConsumer { get; }

        IModelContextSupervisor ModelContextSupervisor { get; }
    }
}
