namespace MassTransit
{
    using System;
    using System.Linq;
    using Conductor.Configuration;
    using Conductor.Configuration.Configurators;
    using Conductor.Configuration.Definition;
    using Conductor.Server;
    using Internals.Extensions;
    using Metadata;
    using Util;


    public static class ConductorConfigurationExtensions
    {
        /// <summary>
        /// Configure a service instance, which supports one or more receive endpoints, all of which are managed by conductor.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ServiceInstance<TEndpointConfigurator>(this IReceiveConfigurator<TEndpointConfigurator> configurator,
            Action<IServiceInstanceConfigurator<TEndpointConfigurator>> configure)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            ServiceInstance(configurator, new ServiceInstanceOptions(), configure);
        }

        /// <summary>
        /// Configure a service instance, which supports one or more receive endpoints, all of which are managed by conductor.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options"></param>
        /// <param name="configure"></param>
        public static void ServiceInstance<TEndpointConfigurator>(this IReceiveConfigurator<TEndpointConfigurator> configurator,
            ServiceInstanceOptions options, Action<IServiceInstanceConfigurator<TEndpointConfigurator>> configure)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            var transportConfigurator = GetServiceInstanceTransportConfigurator<TEndpointConfigurator>();

            var instance = new ServiceInstance();

            if (options.InstanceEndpointEnabled)
            {
                var definition = new InstanceEndpointDefinition(instance);
                configurator.ReceiveEndpoint(definition, options.EndpointNameFormatter, instanceEndpointConfigurator =>
                {
                    var instanceConfigurator = new ServiceInstanceConfigurator<TEndpointConfigurator>(configurator, transportConfigurator, instance, options,
                        instanceEndpointConfigurator);

                    configure?.Invoke(instanceConfigurator);
                });
            }
            else
            {
                var instanceConfigurator = new ServiceInstanceConfigurator<TEndpointConfigurator>(configurator, transportConfigurator, instance, options);

                configure?.Invoke(instanceConfigurator);
            }
        }

        static IServiceInstanceTransportConfigurator<TEndpointConfigurator> GetServiceInstanceTransportConfigurator<TEndpointConfigurator>()
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            var assembly = typeof(TEndpointConfigurator).Assembly;

            var transportConfiguratorType = AssemblyTypeCache
                .FindTypes(assembly, TypeClassification.Concrete, x => x.HasInterface<IServiceInstanceTransportConfigurator<TEndpointConfigurator>>())
                .GetAwaiter().GetResult().FirstOrDefault();

            if (transportConfiguratorType == null)
                throw new ArgumentException($"Type not found: {TypeMetadataCache<IServiceInstanceTransportConfigurator<TEndpointConfigurator>>.ShortName}");

            return (IServiceInstanceTransportConfigurator<TEndpointConfigurator>)Activator.CreateInstance(transportConfiguratorType);
        }
    }
}
