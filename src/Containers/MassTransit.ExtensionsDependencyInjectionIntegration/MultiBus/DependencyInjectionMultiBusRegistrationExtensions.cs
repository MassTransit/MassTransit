namespace MassTransit.MultiBus
{
    using System;
    using System.Linq;
    using ExtensionsDependencyInjectionIntegration.MultiBus;
    using Internals.Reflection;
    using Microsoft.Extensions.DependencyInjection;


    /// <summary>
    /// Support for multiple bus instances in a single container. This is an advanced concept. Review the documentation
    /// for details on the constraints and known limitations of this approach.
    /// </summary>
    public static class DependencyInjectionMultiBusRegistrationExtensions
    {
        /// <summary>
        /// Configure a MassTransit bus instance, using the specified <typeparamref name="TBus" /> bus type, which must inherit directly from <see cref="IBus" />.
        /// A type that implements <typeparamref name="TBus" /> is required, specified by the <typeparamref name="TBusInstance" /> parameter.
        /// </summary>
        /// <param name="collection">The service collection</param>
        /// <param name="configure">Bus instance configuration method</param>
        public static IServiceCollection AddMassTransit<TBus, TBusInstance>(this IServiceCollection collection,
            Action<IServiceCollectionConfigurator<TBus>> configure)
            where TBus : class, IBus
            where TBusInstance : BusInstance<TBus>, TBus
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            if (collection.Any(d => d.ServiceType == typeof(TBus)))
            {
                throw new ConfigurationException(
                    $"AddMassTransit<{typeof(TBus).Name},{typeof(TBusInstance).Name}>() was already called and may only be called once per container. To configure additional bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
            }

            var configurator = new ServiceCollectionBusConfigurator<TBus, TBusInstance>(collection);

            configure(configurator);

            return collection;
        }

        /// <summary>
        /// Configure a MassTransit bus instance, using the specified <typeparamref name="TBus" /> bus type, which must inherit directly from <see cref="IBus" />.
        /// A dynamic type will be created to support the bus instance, which will be initialized when the <typeparamref name="TBus" /> type is retrieved
        /// from the container.
        /// </summary>
        /// <param name="collection">The service collection</param>
        /// <param name="configure">Bus instance configuration method</param>
        public static IServiceCollection AddMassTransit<TBus>(this IServiceCollection collection, Action<IServiceCollectionConfigurator<TBus>> configure)
            where TBus : class, IBus
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var doIt = new Callback<TBus>(collection, configure);

            return BusInstanceBuilder.Instance.GetBusInstanceType(doIt);
        }


        class Callback<TBus> :
            IBusInstanceBuilderCallback<TBus, IServiceCollection>
            where TBus : class, IBus
        {
            readonly Action<IServiceCollectionConfigurator<TBus>> _configure;
            readonly IServiceCollection _services;

            public Callback(IServiceCollection services, Action<IServiceCollectionConfigurator<TBus>> configure)
            {
                _services = services;
                _configure = configure;
            }

            public IServiceCollection GetResult<TBusInstance>()
                where TBusInstance : BusInstance<TBus>, TBus
            {
                return _services.AddMassTransit<TBus, TBusInstance>(_configure);
            }
        }
    }
}
