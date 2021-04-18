namespace MassTransit.GrpcTransport.Integration
{
    public interface IGrpcHost :
        IHost<IGrpcReceiveEndpointConfigurator>
    {
    }
}