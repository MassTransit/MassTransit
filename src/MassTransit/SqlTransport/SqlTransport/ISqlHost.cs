namespace MassTransit.SqlTransport
{
    using Transports;


    public interface ISqlHost :
        IHost<ISqlReceiveEndpointConfigurator>
    {
    }
}
