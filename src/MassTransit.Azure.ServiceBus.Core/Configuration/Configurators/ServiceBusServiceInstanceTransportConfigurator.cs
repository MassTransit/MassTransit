namespace MassTransit.Azure.ServiceBus.Core.Configurators
{
    using Conductor.Configuration.Configurators;


    public class ServiceBusServiceInstanceTransportConfigurator :
        IServiceInstanceTransportConfigurator<IServiceBusReceiveEndpointConfigurator>
    {
        public void ConfigureServiceEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.SubscribeMessageTopics = false;
        }

        public void ConfigureInstanceServiceEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.SubscribeMessageTopics = false;
        }

        public void ConfigureControlEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.RemoveSubscriptions = true;
            configurator.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
        }

        public void ConfigureInstanceEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.RemoveSubscriptions = true;
            configurator.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
        }
    }
}
