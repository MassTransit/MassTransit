namespace MassTransit
{
    public interface IGrpcPublishTopology :
        IPublishTopology
    {
        new IGrpcMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
