namespace MassTransit.Mediator
{
    public interface IMediator :
        ISendEndpoint,
        IPublishEndpoint,
        IPublishEndpointProvider,
        IClientFactory,
        IConsumePipeConnector,
        IRequestPipeConnector,
        IConsumeObserverConnector,
        IConsumeMessageObserverConnector
    {
    }
}
