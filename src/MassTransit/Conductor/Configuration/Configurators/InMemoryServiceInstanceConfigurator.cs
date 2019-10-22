namespace MassTransit.Conductor.Configuration.Configurators
{
    using Server;


    public class InMemoryServiceInstanceConfigurator :
        ServiceInstanceConfigurator<IInMemoryReceiveEndpointConfigurator>
    {
        public InMemoryServiceInstanceConfigurator(IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator> configurator, IServiceInstance instance)
            : base(configurator, instance)
        {
        }
    }
}
