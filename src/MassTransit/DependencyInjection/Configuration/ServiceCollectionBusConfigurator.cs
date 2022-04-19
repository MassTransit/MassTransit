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
    using Transactions;
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
                return new BusRegistrationContext(provider, Registrar);
            }

            collection.AddSingleton(provider => ClientFactoryProvider(provider, provider.GetRequiredService<IBus>()));

            collection.AddSingleton(provider => Bind<IBus>.Create(CreateRegistrationContext(provider)));
            collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, IBusRegistrationContext>>().Value);

            collection.TryAdd(ServiceDescriptor.Singleton(typeof(IReceiveEndpointDispatcher<>), typeof(ReceiveEndpointDispatcher<>)));
            collection.AddSingleton<IReceiveEndpointDispatcherFactory>(provider =>
            {
                var registrationContext = provider.GetRequiredService<Bind<IBus, IBusRegistrationContext>>().Value;
                var busInstance = provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value;

                return new ReceiveEndpointDispatcherFactory(registrationContext, busInstance);
            });
        }

        protected ServiceCollectionBusConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
            : base(collection, registrar)
        {
            AddMassTransitComponents(collection);
        }

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
            var configurator = new ServiceCollectionRiderConfigurator(this, new DependencyInjectionRiderContainerRegistrar(this));
            configure?.Invoke(configurator);
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
            collection.TryAddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext() ?? MissingConsumeContext.Instance);

            collection.TryAddScoped(GetCurrentSendEndpointProvider);
            collection.TryAddScoped(GetCurrentPublishEndpoint);

            collection.TryAddSingleton<IConsumeScopeProvider>(provider => new ConsumeScopeProvider(provider));

            collection.TryAddScoped(typeof(IRequestClient<>), typeof(GenericRequestClient<>));
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IServiceProvider provider)
        {
            var consumeContextProvider = provider.GetRequiredService<ScopedConsumeContextProvider>();
            if (consumeContextProvider.HasContext)
                return consumeContextProvider.GetContext();

            var bus = provider.GetService<ITransactionalBus>() ?? (ISendEndpointProvider)provider.GetRequiredService<IBus>();
            return new ScopedSendEndpointProvider<IServiceProvider>(bus, provider);
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IServiceProvider provider)
        {
            var consumeContextProvider = provider.GetRequiredService<ScopedConsumeContextProvider>();
            if (consumeContextProvider.HasContext)
                return consumeContextProvider.GetContext();

            var bus = provider.GetService<ITransactionalBus>() ?? provider.GetRequiredService<IBus>();
            return new PublishEndpoint(new ScopedPublishEndpointProvider<IServiceProvider>(bus, provider));
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
                return new BusRegistrationContext(provider, Registrar);
            }

            collection.AddScoped(provider => Bind<TBus>.Create(GetSendEndpointProvider(provider)));
            collection.AddScoped(provider => Bind<TBus>.Create(GetPublishEndpoint(provider)));
            collection.AddSingleton(provider => Bind<TBus>.Create(ClientFactoryProvider(provider, provider.GetRequiredService<TBus>())));

            collection.AddSingleton(provider => Bind<TBus>.Create(CreateRegistrationContext(provider)));
        }

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
            this.AddSingleton(provider =>
                Bind<TBus>.Create<IReceiveEndpointConnector>(provider.GetRequiredService<IBusInstance<TBus>>()));
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

        static IBusInstance<TBus> CreateBus<T>(T busFactory, IServiceProvider provider)
            where T : IRegistrationBusFactory
        {
            IEnumerable<IBusInstanceSpecification> specifications = provider.GetServices<Bind<TBus, IBusInstanceSpecification>>().Select(x => x.Value);

            var instance = busFactory.CreateBus(provider.GetRequiredService<Bind<TBus, IBusRegistrationContext>>().Value, specifications, typeof(TBus).Name);

            var busInstance = provider.GetService<TBusInstance>() ?? ActivatorUtilities.CreateInstance<TBusInstance>(provider, instance.BusControl);

            return new MultiBusInstance<TBus>(busInstance, instance);
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
