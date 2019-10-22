namespace MassTransit.Transports.InMemory.Configuration
{
    using MassTransit.Configuration;


    public interface IInMemoryReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IInMemoryEndpointConfiguration
    {
        IInMemoryReceiveEndpointConfigurator Configurator { get; }

        int ConcurrencyLimit { get; }

        void Build(IInMemoryHostControl host);
    }
}
