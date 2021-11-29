namespace MassTransit
{
    using System;
    using Configuration;


    public static class ServiceInstanceConfigurationExtensions
    {
        /// <summary>
        /// Configure a service instance for use with the job service
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ServiceInstance<TEndpointConfigurator>(this IBusFactoryConfigurator<TEndpointConfigurator> configurator,
            Action<IServiceInstanceConfigurator<TEndpointConfigurator>> configure)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            ServiceInstance(configurator, new ServiceInstanceOptions(), configure);
        }

        /// <summary>
        /// Configure a service instance for use with the job service
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options"></param>
        /// <param name="configure"></param>
        public static void ServiceInstance<TEndpointConfigurator>(this IBusFactoryConfigurator<TEndpointConfigurator> configurator,
            ServiceInstanceOptions options, Action<IServiceInstanceConfigurator<TEndpointConfigurator>> configure)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            var definition = new InstanceEndpointDefinition();

            configurator.ReceiveEndpoint(definition, options.EndpointNameFormatter, endpointConfigurator =>
            {
                var instanceConfigurator = new ServiceInstanceConfigurator<TEndpointConfigurator>(configurator, options, endpointConfigurator);

                configure?.Invoke(instanceConfigurator);
            });
        }
    }
}
