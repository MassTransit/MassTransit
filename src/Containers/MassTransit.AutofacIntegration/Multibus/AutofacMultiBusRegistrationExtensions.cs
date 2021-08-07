namespace MassTransit
{
    using System;
    using Autofac;
    using Autofac.Core;
    using AutofacIntegration.MultiBus;
    using Internals.Reflection;
    using MultiBus;


    /// <summary>
    /// Support for multiple bus instances in a single container. This is an advanced concept. Review the documentation
    /// for details on the constraints and known limitations of this approach.
    /// </summary>
    public static class AutofacMultiBusRegistrationExtensions
    {
        public static ContainerBuilder AddMassTransit<TBus, TBusInstance>(this ContainerBuilder containerBuilder,
            Action<IContainerBuilderBusConfigurator<TBus>> configure)
            where TBus : class, IBus
            where TBusInstance : BusInstance<TBus>, TBus
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            if (containerBuilder.ComponentRegistryBuilder.IsRegistered(new TypedService(typeof(TBus))))
            {
                throw new ConfigurationException(
                    $"AddMassTransit<{typeof(TBus).Name},{typeof(TBusInstance).Name}>() was already called and may only be called once per container. To configure additional bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
            }

            var configurator = new ContainerBuilderBusConfigurator<TBus, TBusInstance>(containerBuilder);

            configure(configurator);

            return containerBuilder;
        }

        /// <summary>
        /// Configure a MassTransit bus instance, using the specified <typeparamref name="TBus" /> bus type, which must inherit directly from <see cref="IBus" />.
        /// A dynamic type will be created to support the bus instance, which will be initialized when the <typeparamref name="TBus" /> type is retrieved
        /// from the container.
        /// </summary>
        /// <param name="containerBuilder">The container builder</param>
        /// <param name="configure">Bus instance configuration method</param>
        public static ContainerBuilder AddMassTransit<TBus>(this ContainerBuilder containerBuilder, Action<IContainerBuilderBusConfigurator<TBus>> configure)
            where TBus : class, IBus
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var doIt = new Callback<TBus>(containerBuilder, configure);

            return BusInstanceBuilder.Instance.GetBusInstanceType(doIt);
        }


        class Callback<TBus> :
            IBusInstanceBuilderCallback<TBus, ContainerBuilder>
            where TBus : class, IBus
        {
            readonly Action<IContainerBuilderBusConfigurator<TBus>> _configure;
            readonly ContainerBuilder _services;

            public Callback(ContainerBuilder services, Action<IContainerBuilderBusConfigurator<TBus>> configure)
            {
                _services = services;
                _configure = configure;
            }

            public ContainerBuilder GetResult<TBusInstance>()
                where TBusInstance : BusInstance<TBus>, TBus
            {
                return _services.AddMassTransit<TBus, TBusInstance>(_configure);
            }
        }
    }
}
