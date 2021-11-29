namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;
    using Transports;


    public interface IGrpcReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IGrpcEndpointConfiguration
    {
        IGrpcReceiveEndpointConfigurator Configurator { get; }

        void Build(IHost host);
    }
}
