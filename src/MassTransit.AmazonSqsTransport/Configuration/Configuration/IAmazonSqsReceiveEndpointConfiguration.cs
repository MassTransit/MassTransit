namespace MassTransit.AmazonSqsTransport.Configuration.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public interface IAmazonSqsReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IAmazonSqsEndpointConfiguration
    {
        bool SubscribeMessageTopics { get; set; }

        ReceiveSettings Settings { get; }

        void Build(IAmazonSqsHostControl host);
    }
}
