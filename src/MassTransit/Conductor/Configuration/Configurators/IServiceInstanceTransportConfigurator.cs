namespace MassTransit.Conductor.Configuration.Configurators
{
    public interface IServiceInstanceTransportConfigurator<in TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Called before the service endpoint is configured
        /// </summary>
        /// <param name="configurator"></param>
        void ConfigureServiceEndpoint(TEndpointConfigurator configurator);

        /// <summary>
        /// Called before the instance-specific service endpoint is configured
        /// </summary>
        /// <param name="configurator"></param>
        void ConfigureInstanceServiceEndpoint(TEndpointConfigurator configurator);

        /// <summary>
        /// Called before the control endpoint is configured
        /// </summary>
        /// <param name="configurator"></param>
        void ConfigureControlEndpoint(TEndpointConfigurator configurator);

        /// <summary>
        /// Called before the instance endpoint is configured
        /// </summary>
        /// <param name="configurator"></param>
        void ConfigureInstanceEndpoint(TEndpointConfigurator configurator);
    }
}
