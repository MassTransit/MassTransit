namespace MassTransit.DependencyInjection
{
    public interface ScopedBusContext
    {
        ISendEndpointProvider SendEndpointProvider { get; }
        IPublishEndpoint PublishEndpoint { get; }
        IScopedClientFactory ClientFactory { get; }
    }
}
