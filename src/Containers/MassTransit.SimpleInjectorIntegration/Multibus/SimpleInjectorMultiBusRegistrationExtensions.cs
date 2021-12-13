using MassTransit.MultiBus;
using SimpleInjector;
using System;
using System.Linq;

namespace MassTransit.SimpleInjectorIntegration.Multibus
{
    using Internals.Reflection;


    public static class SimpleInjectorMultiBusRegistrationExtensions
    {
        public static Container AddMassTransit<TBus, TBusInstance>(this Container container,
            Action<ISimpleInjectorBusConfigurator<TBus>> configure)
            where TBus : class, IBus
            where TBusInstance : BusInstance<TBus>, TBus
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            if(container.GetCurrentRegistrations().Any(d => d.ServiceType == typeof(TBus)))
            {
                throw new ConfigurationException(
                    $"AddMassTransit<{typeof(TBus).Name},{typeof(TBusInstance).Name}>() was already called and may only be called once per container. To configure additional bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
            }

            var configurator = new SimpleInjectorBusConfigurator<TBus, TBusInstance>(container);

            configure(configurator);

            return container;
        }

        public static Container AddMassTransit<TBus>(this Container container, Action<ISimpleInjectorBusConfigurator<TBus>> configure)
        where TBus : class, IBus
        {
            if(configure == null)
                throw new ArgumentNullException(nameof(configure));

            var doIt = new CallBack<TBus>(container, configure);

            return BusInstanceBuilder.Instance.GetBusInstanceType(doIt);
        }


        class CallBack<TBus> : IBusInstanceBuilderCallback<TBus, Container>
            where TBus : class, IBus
        {
            readonly Action<ISimpleInjectorBusConfigurator<TBus>> _configure;
            readonly Container _container;

            public CallBack(Container container, Action<ISimpleInjectorBusConfigurator<TBus>> configure)
            {
                _container = container;
                _configure = configure;
            }

            public Container GetResult<TBusInstance>()
                where TBusInstance : BusInstance<TBus>, TBus
            {
                return _container.AddMassTransit<TBus, TBusInstance>(_configure);
            }
        }
    }
}
