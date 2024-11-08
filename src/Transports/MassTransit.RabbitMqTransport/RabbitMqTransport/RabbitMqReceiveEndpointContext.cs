namespace MassTransit.RabbitMqTransport
{
    using Topology;
    using Transports;


    public interface RabbitMqReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        bool ExclusiveConsumer { get; }

        bool IsNotReplyTo { get; }

        IChannelContextSupervisor ChannelContextSupervisor { get; }
    }
}
