namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IGrpcReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IGrpcEndpointConfiguration
    {
        IGrpcReceiveEndpointConfigurator Configurator { get; }

        void Build(IHost host);
    }
}
