namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public class DependencyInjectionContainerRegistrar :
        IContainerRegistrar
    {
        protected readonly IServiceCollection Collection;

        public DependencyInjectionContainerRegistrar(IServiceCollection collection)
        {
            Collection = collection;
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            Collection.TryAddScoped(provider => GetScopedBusContext(provider).CreateRequestClient<T>(timeout));
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            Collection.TryAddScoped(provider => GetScopedBusContext(provider).CreateRequestClient<T>(destinationAddress, timeout));
        }

        public void RegisterScopedClientFactory()
        {
            Collection.TryAddScoped(provider => GetScopedBusContext(provider));
        }

        public virtual void RegisterEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            Collection.TryAddSingleton(endpointNameFormatter);
        }

        public T GetOrAdd<T>(Type type, Func<Type, T> missingRegistrationFactory = default)
            where T : class, IRegistration
        {
            if (TryGetValue<T>(type, out var value))
                return value;

            Func<Type, T> factory = missingRegistrationFactory ?? throw new ArgumentNullException(nameof(missingRegistrationFactory));

            value = factory(type);

            AddRegistration(value);

            return value;
        }

        public virtual IEnumerable<T> GetRegistrations<T>()
            where T : class, IRegistration
        {
            return Collection.Where(x => x.ServiceType == typeof(T)).Select(x => x.ImplementationInstance).Cast<T>();
        }

        public bool TryGetValue<T>(IServiceProvider provider, Type type, out T value)
            where T : class, IRegistration
        {
            value = GetRegistrations<T>(provider).FirstOrDefault(x => x.Type == type);

            return value != null;
        }

        public virtual IEnumerable<T> GetRegistrations<T>(IServiceProvider provider)
            where T : class, IRegistration
        {
            return provider.GetService<IEnumerable<T>>() ?? Array.Empty<T>();
        }

        public IConfigureReceiveEndpoint GetConfigureReceiveEndpoints(IServiceProvider provider)
        {
            IConfigureReceiveEndpoint[] globalConfigureReceiveEndpoints = provider.GetServices<IConfigureReceiveEndpoint>().ToArray();

            return new ConfigureReceiveEndpoint(globalConfigureReceiveEndpoints, GetBusConfigureReceiveEndpoints(provider));
        }

        public virtual IEndpointNameFormatter GetEndpointNameFormatter(IServiceProvider provider)
        {
            return provider.GetService<IEndpointNameFormatter>() ?? DefaultEndpointNameFormatter.Instance;
        }

        protected virtual IConfigureReceiveEndpoint[] GetBusConfigureReceiveEndpoints(IServiceProvider provider)
        {
            return provider.GetServices<Bind<IBus, IConfigureReceiveEndpoint>>().Select(x => x.Value).ToArray();
        }

        bool TryGetValue<T>(Type type, out T value)
            where T : class, IRegistration
        {
            value = GetRegistrations<T>().FirstOrDefault(x => x.Type == type);

            return value != null;
        }

        protected virtual void AddRegistration<T>(T value)
            where T : class, IRegistration
        {
            Collection.Add(ServiceDescriptor.Singleton(value));
        }

        protected virtual IScopedClientFactory GetScopedBusContext(IServiceProvider provider)
        {
            return provider.GetRequiredService<IScopedBusContextProvider<IBus>>().Context.ClientFactory;
        }


        class ConfigureReceiveEndpoint :
            IConfigureReceiveEndpoint
        {
            readonly IConfigureReceiveEndpoint[] _global;
            readonly IConfigureReceiveEndpoint[] _typed;

            public ConfigureReceiveEndpoint(IConfigureReceiveEndpoint[] global, IConfigureReceiveEndpoint[] typed)
            {
                _global = global;
                _typed = typed;
            }

            public void Configure(string name, IReceiveEndpointConfigurator configurator)
            {
                for (var i = 0; i < _global.Length; i++)
                    _global[i].Configure(name, configurator);

                for (var i = 0; i < _typed.Length; i++)
                    _typed[i].Configure(name, configurator);
            }
        }
    }


    public class DependencyInjectionContainerRegistrar<TBus> :
        DependencyInjectionContainerRegistrar
        where TBus : class, IBus
    {
        public DependencyInjectionContainerRegistrar(IServiceCollection collection)
            : base(collection)
        {
        }

        public override IEnumerable<T> GetRegistrations<T>()
        {
            return Collection.Where(x => x.ServiceType == typeof(Bind<TBus, T>))
                .Select(x => x.ImplementationInstance).Cast<Bind<TBus, T>>()
                .Select(x => x.Value);
        }

        public override IEnumerable<T> GetRegistrations<T>(IServiceProvider provider)
        {
            return provider.GetService<IEnumerable<Bind<TBus, T>>>().Select(x => x.Value) ?? Array.Empty<T>();
        }

        protected override void AddRegistration<T>(T value)
        {
            Collection.Add(ServiceDescriptor.Singleton(Bind<TBus>.Create(value)));
        }

        public override void RegisterEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            Collection.TryAddSingleton(Bind<TBus>.Create(endpointNameFormatter));
        }

        protected override IScopedClientFactory GetScopedBusContext(IServiceProvider provider)
        {
            return provider.GetRequiredService<IScopedBusContextProvider<TBus>>().Context.ClientFactory;
        }

        protected override IConfigureReceiveEndpoint[] GetBusConfigureReceiveEndpoints(IServiceProvider provider)
        {
            return provider.GetServices<Bind<TBus, IConfigureReceiveEndpoint>>().Select(x => x.Value).ToArray();
        }

        public override IEndpointNameFormatter GetEndpointNameFormatter(IServiceProvider provider)
        {
            var bind = provider.GetService<Bind<TBus, IEndpointNameFormatter>>();
            return bind != null
                ? bind.Value
                : base.GetEndpointNameFormatter(provider);
        }
    }
}
