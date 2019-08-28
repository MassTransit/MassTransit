namespace MassTransit.Conductor.Configuration
{
    public interface IServiceInstanceConfigurator<out TEndpointConfigurator> :
        IReceiveConfigurator<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
    }
}
