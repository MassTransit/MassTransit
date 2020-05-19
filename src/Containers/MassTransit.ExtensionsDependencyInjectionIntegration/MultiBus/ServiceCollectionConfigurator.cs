namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;
    using Registration;
    using Scoping;
    using Transports;


    public class ServiceCollectionConfigurator<TBus, TBusInstance> :
        ServiceCollectionConfigurator,
        IServiceCollectionConfigurator<TBus>
        where TBus : class, IBus
        where TBusInstance : BusInstance<TBus>, TBus
    {
        public ServiceCollectionConfigurator(IServiceCollection collection)
            : base(collection, new DependencyInjectionContainerRegistrar<TBus>(collection))
        {
            collection.AddSingleton(provider => Bind<TBus>.Create(GetSendEndpointProvider(provider)));
            collection.AddSingleton(provider => Bind<TBus>.Create(GetPublishEndpoint(provider)));
            collection.AddSingleton(provider =>
                Bind<TBus>.Create(ClientFactoryProvider(provider.GetRequiredService<IConfigurationServiceProvider>(), provider.GetRequiredService<TBus>())));

            collection.AddSingleton(provider => Bind<TBus>.Create(new BusHealth(typeof(TBus).Name)));
            collection.AddSingleton<IBusHealth>(provider => provider.GetRequiredService<Bind<TBus, BusHealth>>().Value);

            collection.AddSingleton<BusRegistryInstance<TBus>>();
            collection.AddSingleton<IBusRegistryInstance>(provider => provider.GetRequiredService<BusRegistryInstance<TBus>>());

            collection.AddSingleton(provider => Bind<TBus>.Create(CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>())));
        }

        public override void AddBus(Func<IRegistrationContext<IServiceProvider>, IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured();

            Collection.AddSingleton(provider => Bind<TBus>.Create(BusFactory(provider, busFactory)));

            Collection.AddSingleton<TBus>(provider => ActivatorUtilities.CreateInstance<TBusInstance>(provider,
                provider.GetRequiredService<Bind<TBus, IBusControl>>().Value));

            Collection.AddSingleton(provider => Bind<TBus>.Create<IBus>(provider.GetRequiredService<TBus>()));
        }

        static ISendEndpointProvider GetSendEndpointProvider(IServiceProvider provider)
        {
            return new ScopedSendEndpointProvider<IServiceProvider>(provider.GetRequiredService<TBus>(), provider);
        }

        static IPublishEndpoint GetPublishEndpoint(IServiceProvider provider)
        {
            return new PublishEndpoint(new ScopedPublishEndpointProvider<IServiceProvider>(provider.GetRequiredService<TBus>(), provider));
        }

        protected override IRegistrationContext<IServiceProvider> GetRegistrationContext(IServiceProvider provider)
        {
            return new RegistrationContext<IServiceProvider>(
                CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()),
                provider.GetRequiredService<Bind<TBus, BusHealth>>().Value,
                provider
            );
        }
    }
}
