namespace MassTransit.GrpcTransport
{
    using Transports;


    public interface IGrpcHost :
        IHost<IGrpcReceiveEndpointConfigurator>
    {
    }
}