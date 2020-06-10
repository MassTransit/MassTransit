namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using Transports;


    public class ServiceCollectionBusConfigurator :
        RegistrationConfigurator,
        IServiceCollectionBusConfigurator
    {
        protected readonly HashSet<Type> RiderTypes;

        public ServiceCollectionBusConfigurator(IServiceCollection collection)
            : this(collection, new DependencyInjectionContainerRegistrar(collection))
        {
            IBusRegistrationContext CreateRegistrationContext(IServiceProvider serviceProvider)
            {
                var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();
                var busHealth = serviceProvider.GetRequiredService<BusHealth>();
                return new BusRegistrationContext(provider, busHealth, Endpoints, Consumers, Sagas, ExecuteActivities, Activities);
            }

            collection.AddSingleton(provider => ClientFactoryProvider(provider.GetRequiredService<IConfigurationServiceProvider>(),
                provider.GetRequiredService<IBus>()));

            collection.AddSingleton(provider => new BusHealth(nameof(IBus)));
            collection.AddSingleton<IBusHealth>(provider => provider.GetRequiredService<BusHealth>());

            collection.AddSingleton(provider => Bind<IBus>.Create(CreateRegistrationContext(provider)));
            collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusRegistrationContext>>().Value);
        }

        protected ServiceCollectionBusConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
            : base(registrar)
        {
            Collection = collection;
            RiderTypes = new HashSet<Type>();

            AddMassTransitComponents(collection);
        }

        public IServiceCollection Collection { get; }

        public virtual void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            SetBusFactory(new RegistrationBusFactory(busFactory));
        }

        public virtual void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusFactory));

            Collection.AddSingleton(provider => Bind<IBus>.Create(CreateBus(busFactory, provider)));

            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value);
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.BusControl);
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.Bus);
        }

        public virtual void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            var configurator = new ServiceCollectionRiderConfigurator(Collection, Registrar, RiderTypes);
            configure?.Invoke(configurator);
        }

        static IBusInstance CreateBus<T>(T busFactory, IServiceProvider provider)
            where T : IRegistrationBusFactory
        {
            IEnumerable<IBusInstanceSpecification> specifications = provider.GetServices<Bind<IBus, IBusInstanceSpecification>>().Select(x => x.Value);

            var busInstance = busFactory.CreateBus(provider.GetRequiredService<Bind<IBus, IBusRegistrationContext>>().Value, specifications);

            return busInstance;
        }

        static void AddMassTransitComponents(IServiceCollection collection)
        {
            collection.TryAddSingleton<IBusDepot, BusDepot>();

            collection.TryAddScoped<ScopedConsumeContextProvider>();
            collection.TryAddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext() ?? MissingConsumeContext.Instance);

            collection.TryAddScoped(GetCurrentSendEndpointProvider);
            collection.TryAddScoped(GetCurrentPublishEndpoint);

            collection.TryAddSingleton<IConsumerScopeProvider>(provider => new DependencyInjectionConsumerScopeProvider(provider));
            collection.TryAddSingleton<IConfigurationServiceProvider>(provider => new DependencyInjectionConfigurationServiceProvider(provider));
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IServiceProvider provider)
        {
            return (ISendEndpointProvider)provider.GetService<ScopedConsumeContextProvider>()?.GetContext()
                ?? new ScopedSendEndpointProvider<IServiceProvider>(provider.GetRequiredService<IBus>(), provider);
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IServiceProvider provider)
        {
            return (IPublishEndpoint)provider.GetService<ScopedConsumeContextProvider>()?.GetContext() ?? new PublishEndpoint(
                new ScopedPublishEndpointProvider<IServiceProvider>(provider.GetRequiredService<IBus>(), provider));
        }
    }
}
