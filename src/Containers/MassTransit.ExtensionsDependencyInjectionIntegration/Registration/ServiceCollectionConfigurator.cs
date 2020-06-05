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


    public class ServiceCollectionConfigurator :
        RegistrationConfigurator,
        IServiceCollectionConfigurator
    {
        public ServiceCollectionConfigurator(IServiceCollection collection)
            : this(collection, new DependencyInjectionContainerRegistrar(collection))
        {
            collection.AddSingleton(provider => ClientFactoryProvider(provider.GetRequiredService<IConfigurationServiceProvider>(),
                provider.GetRequiredService<IBus>()));

            collection.AddSingleton(provider => new BusHealth(nameof(IBus)));
            collection.AddSingleton<IBusHealth>(provider => provider.GetRequiredService<BusHealth>());

            collection.AddSingleton(provider => CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()));
        }

        protected ServiceCollectionConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
            : base(registrar)
        {
            Collection = collection;

            AddMassTransitComponents(collection);
        }

        public IServiceCollection Collection { get; }

        public virtual void AddBus(Func<IRegistrationContext<IServiceProvider>, IBusControl> busFactory)
        {
            SetBusFactory(new RegistrationBusFactory<IServiceProvider>(busFactory));
        }

        public virtual void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory<IServiceProvider>
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusFactory));

            Collection.AddSingleton(provider => Bind<IBus>.Create(CreateBus(busFactory, provider)));

            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value);
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.BusControl);
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.Bus);
        }

        public virtual void AddRider(Action<IRiderConfigurator<IServiceProvider>> configure)
        {
            var configurator = new ServiceCollectionRiderConfigurator(Collection, Registrar);
            configure?.Invoke(configurator);
        }

        IBusInstance CreateBus<T>(T busFactory, IServiceProvider provider)
            where T : IRegistrationBusFactory<IServiceProvider>
        {
            IEnumerable<IBusInstanceSpecification> specifications = provider.GetServices<Bind<IBus, IBusInstanceSpecification>>().Select(x => x.Value);

            var busInstance = busFactory.CreateBus(GetRegistrationContext(provider), specifications);

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
            collection.TryAddTransient<IConfigurationServiceProvider>(provider => new DependencyInjectionConfigurationServiceProvider(provider));
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

        IRegistrationContext<IServiceProvider> GetRegistrationContext(IServiceProvider provider)
        {
            return new RegistrationContext<IServiceProvider>(
                CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()),
                provider.GetRequiredService<BusHealth>(),
                provider);
        }
    }
}
