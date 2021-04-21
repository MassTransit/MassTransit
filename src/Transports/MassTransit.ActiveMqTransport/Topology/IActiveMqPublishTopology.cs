namespace MassTransit.ActiveMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IActiveMqPublishTopology :
        IPublishTopology
    {
        new IActiveMqMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;

        string VirtualTopicPrefix { get; }
    }
}
