namespace MassTransit.AmazonSqsTransport.Configuration
{
    using MassTransit.Configuration;
    using Transports;


    public interface IAmazonSqsReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IAmazonSqsEndpointConfiguration
    {
        ReceiveSettings Settings { get; }

        void Build(IHost host);
    }
}
