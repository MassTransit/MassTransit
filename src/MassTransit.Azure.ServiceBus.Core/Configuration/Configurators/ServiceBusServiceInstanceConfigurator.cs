namespace MassTransit.Azure.ServiceBus.Core.Configurators
{
    using System;
    using Conductor.Configuration.Configurators;
    using Conductor.Server;


    public class ServiceBusServiceInstanceConfigurator :
        ServiceInstanceConfigurator<IServiceBusHost, IServiceBusReceiveEndpointConfigurator>
    {
        public ServiceBusServiceInstanceConfigurator(IReceiveConfigurator<IServiceBusHost, IServiceBusReceiveEndpointConfigurator> configurator,
            IServiceBusHost host, IServiceInstance instance)
            : base(configurator, host, instance)
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
