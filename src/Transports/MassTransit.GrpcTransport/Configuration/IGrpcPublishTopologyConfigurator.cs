namespace MassTransit
{
    public interface IGrpcPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IGrpcPublishTopology
    {
        new IGrpcMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
