namespace MassTransit
{
    public interface IMediator :
        ISendEndpoint,
        IPublishEndpoint,
        IClientFactory
    {
    }
}
