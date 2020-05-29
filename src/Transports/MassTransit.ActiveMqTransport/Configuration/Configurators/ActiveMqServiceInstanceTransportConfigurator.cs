namespace MassTransit.ActiveMqTransport.Configurators
{
    using Conductor.Configuration.Configurators;


    public class ActiveMqServiceInstanceTransportConfigurator :
        IServiceInstanceTransportConfigurator<IActiveMqReceiveEndpointConfigurator>
    {
        public void ConfigureServiceEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
        }

        public void ConfigureInstanceServiceEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;
        }

        public void ConfigureControlEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.AutoDelete = true;
        }

        public void ConfigureInstanceEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.AutoDelete = true;
        }
    }
}
