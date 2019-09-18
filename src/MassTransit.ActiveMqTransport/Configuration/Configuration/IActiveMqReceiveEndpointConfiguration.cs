namespace MassTransit.ActiveMqTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public interface IActiveMqReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IActiveMqEndpointConfiguration
    {
        bool BindMessageTopics { get; set; }

        ReceiveSettings Settings { get; }

        void Build(IActiveMqHostControl host);
    }
}
