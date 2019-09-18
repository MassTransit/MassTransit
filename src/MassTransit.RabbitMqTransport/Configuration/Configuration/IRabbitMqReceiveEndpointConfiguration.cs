namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;
    using Transport;


    public interface IRabbitMqReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IRabbitMqEndpointConfiguration
    {
        IRabbitMqReceiveEndpointConfigurator Configurator { get; }

        bool BindMessageExchanges { get; }

        ReceiveSettings Settings { get; }

        void Build(IRabbitMqHostControl host);
    }
}
