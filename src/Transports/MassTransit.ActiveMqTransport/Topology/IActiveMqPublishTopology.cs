namespace MassTransit
{
    using ActiveMqTransport.Topology;


    public interface IActiveMqPublishTopology :
        IPublishTopology
    {
        string VirtualTopicPrefix { get; }

        new IActiveMqMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;

        BrokerTopology GetPublishBrokerTopology();
    }
}
