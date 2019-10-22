namespace MassTransit.ActiveMqTransport.Configurators
{
    using ActiveMqTransport;
    using Conductor.Configuration.Configurators;
    using Conductor.Server;


    public class ActiveMqServiceInstanceConfigurator :
        ServiceInstanceConfigurator<IActiveMqReceiveEndpointConfigurator>
    {
        public ActiveMqServiceInstanceConfigurator(IReceiveConfigurator<IActiveMqReceiveEndpointConfigurator> configurator, IServiceInstance instance)
            : base(configurator, instance)
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
