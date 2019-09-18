namespace MassTransit.Azure.ServiceBus.Core.Configurators
{
    using Conductor.Configuration.Configurators;
    using Conductor.Server;


    public class ServiceBusServiceInstanceConfigurator :
        ServiceInstanceConfigurator<IServiceBusReceiveEndpointConfigurator>
    {
        public ServiceBusServiceInstanceConfigurator(IReceiveConfigurator<IServiceBusReceiveEndpointConfigurator> configurator, IServiceInstance instance)
            : base(configurator, instance)
        {
        }

        public override void ConfigureInstanceEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.RemoveSubscriptions = true;
            configurator.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
        }

        protected override void ConfigureServiceEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.SubscribeMessageTopics = false;
        }
    }
}
