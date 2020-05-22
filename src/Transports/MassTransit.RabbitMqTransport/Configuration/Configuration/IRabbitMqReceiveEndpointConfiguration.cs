namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public interface IRabbitMqReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IRabbitMqEndpointConfiguration
    {
        ReceiveSettings Settings { get; }

        void Build(IHost host);
    }
}
