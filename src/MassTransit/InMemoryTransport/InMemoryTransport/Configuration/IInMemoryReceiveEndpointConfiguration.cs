namespace MassTransit.InMemoryTransport.Configuration
{
    using MassTransit.Configuration;
    using Transports;


    public interface IInMemoryReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IInMemoryEndpointConfiguration
    {
        IInMemoryReceiveEndpointConfigurator Configurator { get; }

        void Build(IHost host);
    }
}
