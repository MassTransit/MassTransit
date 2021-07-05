namespace MassTransit.ActiveMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IActiveMqPublishTopology :
        IPublishTopology
    {
        string VirtualTopicPrefix { get; }

        new IActiveMqMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
