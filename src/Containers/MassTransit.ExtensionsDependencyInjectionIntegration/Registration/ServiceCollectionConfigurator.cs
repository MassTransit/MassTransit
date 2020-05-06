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
        readonly string _name;

        public ServiceCollectionConfigurator(string name, IServiceCollection collection)
            : this(name, collection, new DependencyInjectionContainerRegistrar(name, collection))
        {
        }

        protected ServiceCollectionConfigurator(string name, IServiceCollection collection, IContainerRegistrar registrar)
            : base(registrar)
        {
            _name = name;
            Collection = collection;

            AddMassTransitComponents(collection);

            collection.AddSingleton<IRegistrationConfigurator>(this);
            collection.AddSingleton(provider => CreateRegistration(_name, provider.GetRequiredService<IConfigurationServiceProvider>()));
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

            Collection.AddSingleton(BusFactory);
            Collection.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
            Collection.AddSingleton(provider => ClientFactoryProvider(provider.GetRequiredService<IBus>()));

            Collection.AddScoped(GetCurrentSendEndpointProvider<IBus>);
            Collection.AddScoped(GetCurrentPublishEndpoint<IBus>);
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

                    ConfigureMediator(_name, cfg, provider);
                });
            }

            Collection.TryAddSingleton(MediatorFactory);
            Collection.AddSingleton<IClientFactory>(provider => provider.GetRequiredService<IMediator>());
        }

        void AddMassTransitComponents(IServiceCollection collection)
        {
            collection.TryAddScoped<ScopedConsumeContextProvider>();
            collection.TryAddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext(_name) ?? new MissingConsumeContext());

            collection.TryAddSingleton<Func<string, IConsumerScopeProvider>>(provider => name => new DependencyInjectionConsumerScopeProvider(name, provider));

            collection.TryAddSingleton<IConfigurationServiceProvider>(provider => new DependencyInjectionConfigurationServiceProvider(provider));
        }

        protected ISendEndpointProvider GetCurrentSendEndpointProvider<TBus>(IServiceProvider provider)
            where TBus : IBus
        {
            return (ISendEndpointProvider)provider.GetService<ScopedConsumeContextProvider>()?.GetContext(_name)
                ?? new ScopedSendEndpointProvider<IServiceProvider>(provider.GetRequiredService<TBus>(), provider);
        }

        protected IPublishEndpoint GetCurrentPublishEndpoint<TBus>(IServiceProvider provider)
            where TBus : IBus
        {
            return (IPublishEndpoint)provider.GetService<ScopedConsumeContextProvider>()?.GetContext(_name) ?? new PublishEndpoint(
                new ScopedPublishEndpointProvider<IServiceProvider>(provider.GetRequiredService<TBus>(), provider));
        }
    }


    public class ServiceCollectionConfigurator<TBus> :
        ServiceCollectionConfigurator,
        IServiceCollectionConfigurator<TBus>
        where TBus : class
    {
        public ServiceCollectionConfigurator(string name, IServiceCollection collection)
            : base(name, collection, new DependencyInjectionContainerRegistrar<IClientFactory<TBus>>(name, collection))
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

            Collection.AddSingleton<IBusControl<TBus>>(provider => new BusControl<TBus>(BusFactory(provider)));
            Collection.AddSingleton<IBus<TBus>>(provider => provider.GetRequiredService<IBusControl<TBus>>());
            Collection.AddSingleton<IClientFactory<TBus>>(provider =>
                new ClientFactory<TBus>(ClientFactoryProvider(provider.GetRequiredService<IBus<TBus>>())));

            Collection.AddScoped<ISendEndpointProvider<TBus>>(provider => new SendEndpointProvider<TBus>(GetCurrentSendEndpointProvider<IBus<TBus>>(provider)));
            Collection.AddScoped<IPublishEndpoint<TBus>>(provider => new PublishEndpoint<TBus>(GetCurrentPublishEndpoint<IBus<TBus>>(provider)));
        }
    }
}
