namespace MassTransit.AmazonSqsTransport.Configuration
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
