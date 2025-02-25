namespace MassTransit
{
    using System;
    using System.Linq;
    using Configuration;
    using DependencyInjection;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Monitoring;
    using Transports;


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
        public static IServiceCollection AddMassTransit(this IServiceCollection collection, Action<IBusRegistrationConfigurator> configure = null)
        {
            if (collection.Any(d => d.ServiceType == typeof(IBus)))
            {
                throw new ConfigurationException(
                    "AddMassTransit() was already called and may only be called once per container. To configure additional bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
            }

            AddHostedService(collection);
            AddInstrumentation(collection);

            var configurator = new ServiceCollectionBusConfigurator(collection);

            configure?.Invoke(configurator);

            configurator.Complete();

            return collection;
        }

        /// <summary>
        /// Adds the MassTransit Mediator to the <paramref name="collection" />, and allows consumers, sagas, and activities (which are not supported
        /// by the Mediator) to be configured.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure"></param>
        /// <param name="baseAddress"></param>
        public static IServiceCollection AddMediator(this IServiceCollection collection, Uri baseAddress,
            Action<IMediatorRegistrationConfigurator> configure = null)
        {
            if (collection.Any(d => d.ServiceType == typeof(IMediator)))
                throw new ConfigurationException("AddMediator() was already called and may only be called once per container.");

            var configurator = new ServiceCollectionMediatorConfigurator(collection, baseAddress);

            configure?.Invoke(configurator);

            AddInstrumentation(collection);

            configurator.Complete();

            return collection;
        }

        /// <summary>
        /// Adds the MassTransit Mediator to the <paramref name="collection" />, and allows consumers, sagas, and activities (which are not supported
        /// by the Mediator) to be configured.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure"></param>
        public static IServiceCollection AddMediator(this IServiceCollection collection, Action<IMediatorRegistrationConfigurator> configure = null)
        {
            return AddMediator(collection, null, configure);
        }

        /// <summary>
        /// Configure a MassTransit bus instance, using the specified <typeparamref name="TBus" /> bus type, which must inherit directly from <see cref="IBus" />.
        /// A type that implements <typeparamref name="TBus" /> is required, specified by the <typeparamref name="TBusInstance" /> parameter.
        /// </summary>
        /// <param name="collection">The service collection</param>
        /// <param name="configure">Bus instance configuration method</param>
        public static IServiceCollection AddMassTransit<TBus, TBusInstance>(this IServiceCollection collection,
            Action<IBusRegistrationConfigurator<TBus>> configure)
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

            AddHostedService(collection);
            AddInstrumentation(collection);

            var configurator = new ServiceCollectionBusConfigurator<TBus, TBusInstance>(collection);

            configure?.Invoke(configurator);

            configurator.Complete();

            return collection;
        }

        /// <summary>
        /// Configure a MassTransit MultiBus instance, using the specified <typeparamref name="TBus" /> bus type, which must inherit directly from <see cref="IBus" />.
        /// A dynamic type will be created to support the bus instance, which will be initialized when the <typeparamref name="TBus" /> type is retrieved
        /// from the container.
        /// </summary>
        /// <param name="collection">The service collection</param>
        /// <param name="configure">Bus instance configuration method</param>
        public static IServiceCollection AddMassTransit<TBus>(this IServiceCollection collection, Action<IBusRegistrationConfigurator<TBus>> configure)
            where TBus : class, IBus
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var doIt = new Callback<TBus>(collection, configure);

            BusInstanceBuilder.Instance.GetBusInstanceType(doIt);

            return collection;
        }

        /// <summary>
        /// In some situations, it may be necessary to Remove the MassTransitHostedService from the container, such as
        /// when using older versions of the Azure Functions runtime.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RemoveMassTransitHostedService(this IServiceCollection services)
        {
            return RemoveHostedService<MassTransitHostedService>(services);
        }

        /// <summary>
        /// Remove the specified hosted service from the service collection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RemoveHostedService<T>(this IServiceCollection services)
            where T : IHostedService
        {
            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IHostedService) && x.ImplementationType == typeof(T));
            if (descriptor != null)
                services.Remove(descriptor);

            return services;
        }

        /// <summary>
        /// Replace a scoped service registration with a new one
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void ReplaceScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));
        }

        static void AddInstrumentation(IServiceCollection collection)
        {
            collection.AddOptions<InstrumentationOptions>();
            collection.AddSingleton<IConfigureOptions<InstrumentationOptions>, ConfigureDefaultInstrumentationOptions>();
        }

        static void AddHostedService(IServiceCollection collection)
        {
            collection.AddOptions();
            collection.AddHealthChecks();
            collection.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<HealthCheckServiceOptions>, ConfigureBusHealthCheckServiceOptions>());

            collection.AddOptions<MassTransitHostOptions>();
            collection.TryAddSingleton<IValidateOptions<MassTransitHostOptions>, ValidateMassTransitHostOptions>();
            collection.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, MassTransitHostedService>());
        }

        internal static void RemoveMassTransit(this IServiceCollection collection)
        {
            collection.RemoveAll<IClientFactory>();
            collection.RemoveAll<Bind<IBus, IBusRegistrationContext>>();
            collection.RemoveAll<IBusRegistrationContext>();
            collection.RemoveAll(typeof(IReceiveEndpointDispatcher<>));
            collection.RemoveAll<IReceiveEndpointDispatcherFactory>();


            collection.RemoveAll<IBusDepot>();
            collection.RemoveAll<IScopedConsumeContextProvider>();
            collection.RemoveAll<Bind<IBus, ISetScopedConsumeContext>>();
            collection.RemoveAll<Bind<IBus, IScopedConsumeContextProvider>>();
            collection.RemoveAll<IScopedBusContextProvider<IBus>>();
            collection.RemoveAll<ConsumeContext>();
            collection.RemoveAll<ISendEndpointProvider>();
            collection.RemoveAll<IPublishEndpoint>();
            collection.RemoveAll(typeof(IRequestClient<>));
            collection.RemoveAll<IMessageScheduler>();

            collection.RemoveAll<Bind<IBus, IBusInstance>>();
            collection.RemoveAll<IBusInstance>();
            collection.RemoveAll<IReceiveEndpointConnector>();
            collection.RemoveAll<IBusControl>();
            collection.RemoveAll<IBus>();

            collection.RemoveAll<IScopedClientFactory>();
        }


        class Callback<TBus> :
            IBusInstanceBuilderCallback<TBus, IServiceCollection>
            where TBus : class, IBus
        {
            readonly Action<IBusRegistrationConfigurator<TBus>> _configure;
            readonly IServiceCollection _services;

            public Callback(IServiceCollection services, Action<IBusRegistrationConfigurator<TBus>> configure)
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
