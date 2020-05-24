namespace MassTransit.Mediator
{
    using Pipeline;


    public interface IMediator :
        ISendEndpoint,
        IPublishEndpoint,
        IClientFactory,
        IConsumePipeConnector,
        IRequestPipeConnector
    {
    }
}
