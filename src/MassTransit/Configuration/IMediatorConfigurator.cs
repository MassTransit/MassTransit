namespace MassTransit
{
    public interface IMediatorConfigurator :
        IReceiveEndpointConfigurator,
        IConsumeObserverConnector,
        ISendObserverConnector,
        IPublishObserverConnector
    {
    }
}
