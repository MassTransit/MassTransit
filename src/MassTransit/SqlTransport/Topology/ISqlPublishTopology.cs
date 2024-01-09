namespace MassTransit
{
    using SqlTransport.Topology;


    public interface ISqlPublishTopology :
        IPublishTopology
    {
        new ISqlMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;

        BrokerTopology GetPublishBrokerTopology();
    }
}
