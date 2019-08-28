namespace MassTransit.ActiveMqTransport.Configurators
{
    using ActiveMqTransport;
    using Conductor.Configuration.Configurators;
    using Conductor.Server;


    public class ActiveMqServiceInstanceConfigurator :
        ServiceInstanceConfigurator<IActiveMqHost, IActiveMqReceiveEndpointConfigurator>
    {
        public ActiveMqServiceInstanceConfigurator(IReceiveConfigurator<IActiveMqHost, IActiveMqReceiveEndpointConfigurator> configurator,
            IActiveMqHost host, IServiceInstance instance)
            : base(configurator, host, instance)
        {
        }

        public override void ConfigureInstanceEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.AutoDelete = true;
        }

        protected override void ConfigureServiceEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.BindMessageTopics = false;
        }
    }
}
