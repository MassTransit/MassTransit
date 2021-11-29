namespace MassTransit
{
    public delegate void RequestPipeConfiguratorCallback<TRequest>(IRequestPipeConfigurator<TRequest> configurator)
        where TRequest : class;
}
