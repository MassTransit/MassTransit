namespace MassTransit
{
    public interface IGrpcBusTopology :
        IBusTopology
    {
        new IGrpcMessagePublishTopology<T> Publish<T>()
            where T : class;
    }
}
