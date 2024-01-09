namespace MassTransit.SqlTransport.Configuration
{
    using MassTransit.Configuration;
    using Transports;


    public interface ISqlReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        ISqlEndpointConfiguration
    {
        ReceiveSettings Settings { get; }

        void Build(IHost host);
    }
}
