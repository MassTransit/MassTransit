namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using Context;
    using MassTransit.Registration;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using ScopeProviders;
    using Scoping;
    using Transports;


    public class ServiceCollectionConfigurator :
        RegistrationConfigurator,
        IServiceCollectionConfigurator
    {
        protected readonly string Name;

        public ServiceCollectionConfigurator(string name, IServiceCollection collection)
            : base(new DependencyInjectionContainerRegistrar(name, collection))
        {
            Name = name;
            Collection = collection;

            AddMassTransitComponents(collection);

            collection.AddSingleton<IRegistrationConfigurator>(this);
            collection.AddSingleton(provider => CreateRegistration(Name, provider.GetRequiredService<IConfigurationServiceProvider>()));
        }

        public IServiceCollection Collection { get; }

        public virtual void AddBus(Func<IServiceProvider, IBusControl> busFactory)
        {
            IBusControl BusFactory(IServiceProvider serviceProvider)
            {
                var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return busFactory(serviceProvider);
            }

            Collection.AddSingleton<IComponentRegistry>(provider => new ComponentRegistry(Name, BusFactory(provider)));
            Collection.AddSingleton(provider =>
            {
                var componentFactory = provider.GetRequiredService<INamedComponentFactory>();
                return componentFactory.GetBus(Name);
            });
            Collection.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
            Collection.AddSingleton(provider => ClientFactoryProvider(provider.GetRequiredService<IBus>()));

            Collection.AddScoped(GetCurrentSendEndpointProvider);
            Collection.AddScoped(GetCurrentPublishEndpoint);
        }

        public void AddMediator(Action<IServiceProvider, IReceiveEndpointConfigurator> configure = null)
        {
            IMediator MediatorFactory(IServiceProvider serviceProvider)
            {
                var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return Bus.Factory.CreateMediator(cfg =>
                {
                    configure?.Invoke(serviceProvider, cfg);

                    ConfigureMediator(Name, cfg, provider);
                });
            }

            Collection.TryAddSingleton(MediatorFactory);
            Collection.AddSingleton<IClientFactory>(provider => provider.GetRequiredService<IMediator>());
        }

        void AddMassTransitComponents(IServiceCollection collection)
        {
            collection.TryAddSingleton<INamedComponentFactory, NamedComponentFactory>();
            collection.TryAddScoped<ScopedConsumeContextProvider>();
            collection.TryAddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext(Name) ?? new MissingConsumeContext());
            collection.TryAddSingleton<Func<string, IConsumerScopeProvider>>(provider => name => new DependencyInjectionConsumerScopeProvider(name, provider));

            collection.TryAddSingleton<IConfigurationServiceProvider>(provider => new DependencyInjectionConfigurationServiceProvider(provider));
        }

        protected ISendEndpointProvider GetCurrentSendEndpointProvider(IServiceProvider provider)
        {
            return (ISendEndpointProvider)provider.GetService<ScopedConsumeContextProvider>()?.GetContext(Name)
                ?? new ScopedSendEndpointProvider<IServiceProvider>(provider.GetRequiredService<INamedComponentFactory>().GetBus(Name), provider);
        }

        protected IPublishEndpoint GetCurrentPublishEndpoint(IServiceProvider provider)
        {
            return (IPublishEndpoint)provider.GetService<ScopedConsumeContextProvider>()?.GetContext(Name) ?? new PublishEndpoint(
                new ScopedPublishEndpointProvider<IServiceProvider>(provider.GetRequiredService<INamedComponentFactory>().GetBus(Name), provider));
        }
    }


    public class ServiceCollectionConfigurator<TBus> :
        ServiceCollectionConfigurator,
        IServiceCollectionConfigurator<TBus>
        where TBus : class
    {
        public ServiceCollectionConfigurator(IServiceCollection collection)
            : base(typeof(TBus).Name, collection)
        {
        }

        public override void AddBus(Func<IServiceProvider, IBusControl> busFactory)
        {
            IBusControl BusFactory(IServiceProvider serviceProvider)
            {
                var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return busFactory(serviceProvider);
            }

            Collection.AddSingleton<IComponentRegistry>(provider => new ComponentRegistry(Name, BusFactory(provider)));
            Collection.AddSingleton<IBusControl<TBus>>(provider =>
            {
                var componentFactory = provider.GetRequiredService<INamedComponentFactory>();
                return new BusControl<TBus>(componentFactory.GetBus(Name));
            });
            Collection.AddSingleton<IBus<TBus>>(provider => provider.GetRequiredService<IBusControl<TBus>>());
            Collection.AddSingleton<IClientFactory<TBus>>(provider =>
                new ClientFactory<TBus>(ClientFactoryProvider(provider.GetRequiredService<IBus<TBus>>())));

            Collection.AddScoped<ISendEndpointProvider<TBus>>(provider => new SendEndpointProvider<TBus>(GetCurrentSendEndpointProvider(provider)));
            Collection.AddScoped<IPublishEndpoint<TBus>>(provider => new PublishEndpoint<TBus>(GetCurrentPublishEndpoint(provider)));
        }
    }
}
