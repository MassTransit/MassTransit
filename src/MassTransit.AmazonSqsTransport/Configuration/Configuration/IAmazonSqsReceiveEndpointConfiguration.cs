namespace MassTransit.AmazonSqsTransport.Configuration.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public interface IAmazonSqsReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IAmazonSqsEndpointConfiguration
    {
        ReceiveSettings Settings { get; }

        void Build(IAmazonSqsHostControl host);
    }
}
