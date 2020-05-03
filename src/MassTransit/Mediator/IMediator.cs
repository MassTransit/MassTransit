namespace MassTransit.Mediator
{
    public interface IMediator :
        ISendEndpoint,
        IPublishEndpoint,
        IClientFactory
    {
    }
}
