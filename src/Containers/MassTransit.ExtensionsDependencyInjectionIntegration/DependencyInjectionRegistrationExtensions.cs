namespace MassTransit
{
    using System;
    using System.Linq;
    using ExtensionsDependencyInjectionIntegration;
    using ExtensionsDependencyInjectionIntegration.Registration;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;


    /// <summary>
    /// Standard registration extensions, which are used to configure consumers, sagas, and activities on receive endpoints from a
    /// dependency injection container.
    /// </summary>
    public static class DependencyInjectionRegistrationExtensions
    {
        /// <summary>
        /// Adds MassTransit and its dependencies to the <paramref name="collection" />, and allows consumers, sagas, and activities to be configured
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure"></param>
        public static IServiceCollection AddMassTransit(this IServiceCollection collection, Action<IServiceCollectionBusConfigurator> configure = null)
        {
            if (collection.Any(d => d.ServiceType == typeof(IRegistration)))
            {
                throw new ConfigurationException(
                    "AddMassTransit() was already called and may only be called once per container. To configure additional bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
            }

            var configurator = new ServiceCollectionBusConfigurator(collection);

            configure?.Invoke(configurator);

            return collection;
        }

        /// <summary>
        /// Adds the MassTransit Mediator to the <paramref name="collection" />, and allows consumers, sagas, and activities (which are not supported
        /// by the Mediator) to be configured.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure"></param>
        public static IServiceCollection AddMediator(this IServiceCollection collection, Action<IServiceCollectionMediatorConfigurator> configure = null)
        {
            if (collection.Any(d => d.ServiceType == typeof(IMediator)))
                throw new ConfigurationException("AddMediator() was already called and may only be called once per container.");

            var configurator = new ServiceCollectionMediatorConfigurator(collection);

            configure?.Invoke(configurator);

            return collection;
        }
    }
}
