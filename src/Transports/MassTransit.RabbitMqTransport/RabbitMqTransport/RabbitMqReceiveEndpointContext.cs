namespace MassTransit.RabbitMqTransport
{
    using Topology;
    using Transports;


    public interface RabbitMqReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        bool ExclusiveConsumer { get; }

        IModelContextSupervisor ModelContextSupervisor { get; }
    }
}
