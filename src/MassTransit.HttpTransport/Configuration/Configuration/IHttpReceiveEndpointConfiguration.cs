namespace MassTransit.HttpTransport.Configuration
{
    using MassTransit.Configuration;
    using Transport;


    public interface IHttpReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IHttpEndpointConfiguration
    {
        void Build(IHttpHostControl host);
    }
}
