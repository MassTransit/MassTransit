namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using DependencyInjection;
    using DependencyInjection.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Transports;


    public class ServiceCollectionBusConfigurator :
        RegistrationConfigurator,
        IBusRegistrationConfigurator
    {
        public ServiceCollectionBusConfigurator(IServiceCollection collection)
            : this(collection, new DependencyInjectionContainerRegistrar(collection))
        {
            IBusRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var setter = provider.GetRequiredService<Bind<IBus, ISetScopedConsumeContext>>();
                return new BusRegistrationContext(provider, Registrar, setter.Value);
            }

            static Bind<IBus, IScopedConsumeContextProvider> CreateScopeProvider(IServiceProvider provider)
            {
                var global = provider.GetRequiredService<IScopedConsumeContextProvider>();
                return Bind<IBus>.Create((IScopedConsumeContextProvider)new TypedScopedConsumeContextProvider(global));
            }

            collection.AddScoped(CreateScopeProvider);
            collection.AddSingleton(_ =>
                Bind<IBus>.Create((ISetScopedConsumeContext)new SetScopedConsumeContext(provider =>
                    provider.GetRequiredService<Bind<IBus, IScopedConsumeContextProvider>>().Value)));

            collection.AddSingleton(provider => Bind<IBus>.Create(CreateRegistrationContext(provider)));
            collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusRegistrationContext>>().Value);

            collection.TryAdd(ServiceDescriptor.Singleton(typeof(IReceiveEndpointDispatcher<>), typeof(ReceiveEndpointDispatcher<>)));
            collection.TryAddSingleton<IReceiveEndpointDispatcherFactory>(provider =>
            {
                var context = provider.GetRequiredService<Bind<IBus, IBusRegistrationContext>>().Value;
                var busInstance = provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value;

                return new ReceiveEndpointDispatcherFactory(context, busInstance);
            });

            collection.TryAddSingleton(provider => Bind<IBus>.Create(provider.GetRequiredService<IBus>().CreateClientFactory()));
            collection.TryAddSingleton(provider => provider.GetRequiredService<Bind<IBus, IClientFactory>>().Value);

            collection.TryAddScoped<IScopedBusContextProvider<IBus>, ScopedBusContextProvider<IBus>>();
            collection.TryAddScoped(provider => provider.GetRequiredService<IScopedBusContextProvider<IBus>>().Context.SendEndpointProvider);
            collection.TryAddScoped(provider => provider.GetRequiredService<IScopedBusContextProvider<IBus>>().Context.PublishEndpoint);
        }

        protected ServiceCollectionBusConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
            : base(collection, registrar)
        {
            AddMassTransitComponents(collection);
        }

        [Obsolete("This method is deprecated, please use 'Using[TransportName]' instead", true)]
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

            this.AddSingleton(provider => Bind<IBus>.Create(CreateBus(busFactory, provider)));

            this.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value);
            this.AddSingleton<IReceiveEndpointConnector>(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value);
            this.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.BusControl);
            this.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.Bus);

            Registrar.RegisterScopedClientFactory();
        }

        public virtual void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            var configurator = new ServiceCollectionRiderConfigurator(this, new DependencyInjectionRiderContainerRegistrar<IBus>(this));
            configure?.Invoke(configurator);
        }

        public virtual void AddConfigureEndpointsCallback(ConfigureEndpointsCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this.AddSingleton(provider => Bind<IBus>.Create((IConfigureReceiveEndpoint)new ConfigureReceiveEndpointDelegate(callback)));
        }

        public virtual void AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this.AddSingleton(provider => Bind<IBus>.Create((IConfigureReceiveEndpoint)new ConfigureReceiveEndpointDelegateProvider(
                provider.GetRequiredService<Bind<IBus, IBusRegistrationContext>>().Value, callback)));
        }

        static IBusInstance CreateBus<T>(T busFactory, IServiceProvider provider)
            where T : IRegistrationBusFactory
        {
            IEnumerable<IBusInstanceSpecification> specifications = provider.GetServices<Bind<IBus, IBusInstanceSpecification>>().Select(x => x.Value);

            var busInstance = busFactory.CreateBus(provider.GetRequiredService<Bind<IBus, IBusRegistrationContext>>().Value, specifications, string.Empty);

            return busInstance;
        }

        static void AddMassTransitComponents(IServiceCollection collection)
        {
            collection.TryAddSingleton<IBusDepot, BusDepot>();

            collection.TryAddScoped<ScopedConsumeContextProvider>();
            collection.TryAddScoped<IScopedConsumeContextProvider>(provider => provider.GetRequiredService<ScopedConsumeContextProvider>());

            collection.TryAddScoped(provider => provider.GetRequiredService<IScopedConsumeContextProvider>().GetContext() ?? MissingConsumeContext.Instance);

            collection.TryAddScoped(typeof(IRequestClient<>), typeof(GenericRequestClient<>));
        }
    }


    public class ServiceCollectionBusConfigurator<TBus, TBusInstance> :
        ServiceCollectionBusConfigurator,
        IBusRegistrationConfigurator<TBus>
        where TBus : class, IBus
        where TBusInstance : BusInstance<TBus>, TBus
    {
        public ServiceCollectionBusConfigurator(IServiceCollection collection)
            : base(collection, new DependencyInjectionContainerRegistrar<TBus>(collection))
        {
            IBusRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var setter = provider.GetRequiredService<Bind<TBus, ISetScopedConsumeContext>>();
                return new BusRegistrationContext(provider, Registrar, setter.Value);
            }

            static Bind<TBus, IScopedConsumeContextProvider> CreateScopeProvider(IServiceProvider provider)
            {
                var global = provider.GetRequiredService<IScopedConsumeContextProvider>();
                return Bind<TBus>.Create((IScopedConsumeContextProvider)new TypedScopedConsumeContextProvider(global));
            }

            collection.TryAddScoped(CreateScopeProvider);

            collection.AddSingleton(_ =>
                Bind<TBus>.Create((ISetScopedConsumeContext)new SetScopedConsumeContext(provider =>
                    provider.GetRequiredService<Bind<TBus, IScopedConsumeContextProvider>>().Value)));
            collection.TryAddSingleton(provider => Bind<TBus>.Create(provider.GetRequiredService<TBus>().CreateClientFactory()));
            collection.TryAddScoped<IScopedBusContextProvider<TBus>, ScopedBusContextProvider<TBus>>();
            collection.TryAddScoped(provider => Bind<TBus>.Create(provider.GetRequiredService<IScopedBusContextProvider<TBus>>().Context.SendEndpointProvider));
            collection.TryAddScoped(provider => Bind<TBus>.Create(provider.GetRequiredService<IScopedBusContextProvider<TBus>>().Context.PublishEndpoint));

            collection.AddSingleton(provider => Bind<TBus>.Create(CreateRegistrationContext(provider)));
        }

        [Obsolete("This method is deprecated, please use 'Using[TransportName]' instead", true)]
        public override void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            SetBusFactory(new RegistrationBusFactory(busFactory));
        }

        public override void SetBusFactory<T>(T busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusFactory));

            this.AddSingleton(provider => CreateBus(busFactory, provider));

            this.AddSingleton<IBusInstance>(provider => provider.GetRequiredService<IBusInstance<TBus>>());
            this.AddSingleton(provider => Bind<TBus>.Create<IReceiveEndpointConnector>(provider.GetRequiredService<IBusInstance<TBus>>()));
            this.AddSingleton(provider => provider.GetRequiredService<IBusInstance<TBus>>().Bus);

            Registrar.RegisterScopedClientFactory();
        }

        public override void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            AddRider(configurator => configure.Invoke(configurator));
        }

        public void AddRider(Action<IRiderRegistrationConfigurator<TBus>> configure)
        {
            var configurator = new ServiceCollectionRiderConfigurator<TBus>(this, new DependencyInjectionRiderContainerRegistrar<TBus>(this));
            configure?.Invoke(configurator);
        }

        public override void AddConfigureEndpointsCallback(ConfigureEndpointsCallback callback)
        {
            this.AddSingleton(provider => Bind<TBus>.Create((IConfigureReceiveEndpoint)new ConfigureReceiveEndpointDelegate(callback)));
        }

        public override void AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this.AddSingleton(provider => Bind<TBus>.Create((IConfigureReceiveEndpoint)new ConfigureReceiveEndpointDelegateProvider(
                provider.GetRequiredService<Bind<TBus, IBusRegistrationContext>>().Value,
                callback)));
        }

        static IBusInstance<TBus> CreateBus<T>(T busFactory, IServiceProvider provider)
            where T : IRegistrationBusFactory
        {
            IEnumerable<IBusInstanceSpecification> specifications = provider.GetServices<Bind<TBus, IBusInstanceSpecification>>().Select(x => x.Value);

            var instance = busFactory.CreateBus(provider.GetRequiredService<Bind<TBus, IBusRegistrationContext>>().Value, specifications, typeof(TBus).Name);

            var busInstance = provider.GetService<TBusInstance>() ?? ActivatorUtilities.CreateInstance<TBusInstance>(provider, instance.BusControl);

            return new MultiBusInstance<TBus>(busInstance, instance);
        }
    }
}
