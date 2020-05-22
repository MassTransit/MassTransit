namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using Context;
    using GreenPipes;
    using Topology;


    public interface ServiceBusReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        IRetryPolicy RetryPolicy { get; }
    }
}
