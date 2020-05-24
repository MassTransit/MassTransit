namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
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

            ThrowIfAlreadyConfigured();

            Collection.AddSingleton(provider =>
            {
                var busInstance = busFactory.CreateBus(GetRegistrationContext(provider));

                IEnumerable<IBusInstanceConfigurator> configurators = provider.GetServices<Bind<IBus, IBusInstanceConfigurator>>().Select(x => x.Value);

                BusConfigurationResult.CompileResults(configurators.SelectMany(x => x.Validate()));

                foreach (var configurator in configurators)
                    configurator.Configure(busInstance);

                return Bind<IBus>.Create(busInstance);
            });

            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value);
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.BusControl);
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.Bus);
        }

        static void AddMassTransitComponents(IServiceCollection collection)
        {
            collection.TryAddSingleton<IBusDepot, BusDepot>();

            collection.TryAddScoped<ScopedConsumeContextProvider>();
            collection.TryAddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext() ?? new MissingConsumeContext());

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

        IRegistrationContext<IServiceProvider> GetRegistrationContext(IServiceProvider provider)
        {
            return new RegistrationContext<IServiceProvider>(
                CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()),
                provider.GetRequiredService<BusHealth>(),
                provider);
        }
    }
}
