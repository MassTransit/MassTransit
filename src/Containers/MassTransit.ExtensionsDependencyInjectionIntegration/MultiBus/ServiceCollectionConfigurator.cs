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
            Collection.AddSingleton(provider => Bind<TBus>.Create<IRegistrationConfigurator>(this));
            Collection.AddSingleton(provider => Bind<TBus>.Create(CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>())));
        }

        public void AddBus(Func<IRegistrationContext<TBus, IServiceProvider>, IBusControl> busFactory)
        {
            IBusControl BusFactory(IServiceProvider serviceProvider)
            {
                var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                var context = serviceProvider.GetRequiredService<IRegistrationContext<TBus, IServiceProvider>>();

                return busFactory(context);
            }

            Collection.AddSingleton<IRegistrationContext<TBus, IServiceProvider>>(provider => new RegistrationContext<TBus, IServiceProvider>(
                provider.GetRequiredService<Bind<TBus, IRegistration>>(),
                provider.GetRequiredService<Bind<TBus, BusHealth>>(),
                provider));

            Collection.AddSingleton(provider => Bind<TBus>.Create(BusFactory(provider)));

            Collection.AddSingleton<TBus>(provider => ActivatorUtilities.CreateInstance<TBusInstance>(provider,
                provider.GetRequiredService<Bind<TBus, IBusControl>>().Value));

            Collection.AddSingleton(provider => Bind<TBus>.Create<IBus>(provider.GetRequiredService<TBus>()));
            Collection.AddSingleton(provider => Bind<TBus>.Create(GetSendEndpointProvider(provider)));
            Collection.AddSingleton(provider => Bind<TBus>.Create(GetPublishEndpoint(provider)));
            Collection.AddSingleton(provider => Bind<TBus>.Create(ClientFactoryProvider(
                provider.GetRequiredService<IConfigurationServiceProvider>(), provider.GetRequiredService<TBus>())));

            Collection.AddSingleton(provider => Bind<TBus>.Create(new BusHealth(typeof(TBus).Name)));
            Collection.AddSingleton<IBusHealth>(provider => provider.GetRequiredService<Bind<TBus, BusHealth>>().Value);

            Collection.AddSingleton<BusRegistryInstance<TBus>>();
            Collection.AddSingleton<IBusRegistryInstance>(provider => provider.GetRequiredService<BusRegistryInstance<TBus>>());

            Collection.AddScoped(GetSendEndpointProvider);
            Collection.AddScoped(GetPublishEndpoint);
        }

        static ISendEndpointProvider GetSendEndpointProvider(IServiceProvider provider)
        {
            return new ScopedSendEndpointProvider<IServiceProvider>(provider.GetRequiredService<TBus>(), provider);
        }

        static IPublishEndpoint GetPublishEndpoint(IServiceProvider provider)
        {
            return new PublishEndpoint(new ScopedPublishEndpointProvider<IServiceProvider>(provider.GetRequiredService<TBus>(), provider));
        }
    }
}
