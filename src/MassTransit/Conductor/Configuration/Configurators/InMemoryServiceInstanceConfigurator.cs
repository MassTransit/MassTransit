namespace MassTransit.Conductor.Configuration.Configurators
{
    using Server;
    using Transports.InMemory;


    public class InMemoryServiceInstanceConfigurator :
        ServiceInstanceConfigurator<IInMemoryHost, IInMemoryReceiveEndpointConfigurator>
    {
        public InMemoryServiceInstanceConfigurator(IReceiveConfigurator<IInMemoryHost, IInMemoryReceiveEndpointConfigurator> configurator, IInMemoryHost host,
            IServiceInstance instance)
            : base(configurator, host, instance)
        {
        }
    }
}
