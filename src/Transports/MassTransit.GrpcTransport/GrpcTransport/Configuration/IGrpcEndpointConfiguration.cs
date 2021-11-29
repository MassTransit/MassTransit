namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IGrpcEndpointConfiguration :
        IEndpointConfiguration
    {
        new IGrpcTopologyConfiguration Topology { get; }
    }
}
