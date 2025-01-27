#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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

        public virtual void RegisterScopedClientFactory()
        {
            Collection.TryAddScoped(provider => GetScopedBusContext(provider));
        }

        public virtual void RegisterEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            Collection.TryAddSingleton(endpointNameFormatter);
        }

        public T GetOrAddRegistration<T>(Type type, Func<Type, T>? missingRegistrationFactory = default)
            where T : class, IRegistration
        {
            if (TryGetRegistration<T>(type, out var value))
                return value;

            Func<Type, T> factory = missingRegistrationFactory ?? throw new ArgumentNullException(nameof(missingRegistrationFactory));

            value = factory(type);

            AddRegistration(value);

            return value;
        }

        public virtual void AddDefinition<T, TDefinition>()
            where T : class, IDefinition
            where TDefinition : class, T
        {
            Collection.AddSingleton<TDefinition>();
            Collection.AddSingleton<T>(provider => ActivatorUtilities.CreateInstance<TDefinition>(provider));
        }

        public virtual void AddEndpointDefinition<T, TDefinition>(IEndpointSettings<IEndpointDefinition<T>>? settings)
            where T : class
            where TDefinition : class, IEndpointDefinition<T>
        {
            Collection.AddSingleton<TDefinition>();

            if (settings == null)
                Collection.AddSingleton<IEndpointDefinition<T>, TDefinition>();
            else
            {
                Collection.TryAddTransient<IEndpointSettings<IEndpointDefinition<T>>>(
                    _ => throw new InvalidOperationException("The settings are no longer configured in the container."));
                Collection.AddSingleton<IEndpointDefinition<T>>(provider => ActivatorUtilities.CreateInstance<TDefinition>(provider, settings));
            }
        }

        public virtual IEnumerable<T> GetRegistrations<T>()
            where T : class, IRegistration
        {
            return Collection.Where(x => x.ServiceType == typeof(T)).Select(x => x.ImplementationInstance).Cast<T>();
        }

        public bool TryGetRegistration<T>(IServiceProvider provider, Type type, [NotNullWhen(true)] out T? value)
            where T : class, IRegistration
        {
            value = GetRegistrations<T>(provider).FirstOrDefault(x => x.Type == type);

            return value != null;
        }

        public virtual IEnumerable<T> GetRegistrations<T>(IServiceProvider provider)
            where T : class, IRegistration
        {
            return provider.GetServices<T>();
        }

        public virtual T? GetDefinition<T>(IServiceProvider provider)
            where T : class, IDefinition
        {
            return provider.GetService<T>();
        }

        public virtual IEndpointDefinition<T>? GetEndpointDefinition<T>(IServiceProvider provider)
            where T : class
        {
            return provider.GetService<IEndpointDefinition<T>>();
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

        bool TryGetRegistration<T>(Type type, [NotNullWhen(true)] out T? value)
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
            return provider.GetServices<Bind<TBus, T>>().Select(x => x.Value);
        }

        protected override void AddRegistration<T>(T value)
        {
            Collection.Add(ServiceDescriptor.Singleton(Bind<TBus>.Create(value)));
        }

        public override T? GetDefinition<T>(IServiceProvider provider)
            where T : class
        {
            return provider.GetService<Bind<TBus, T>>()?.Value;
        }

        public override IEndpointDefinition<T>? GetEndpointDefinition<T>(IServiceProvider provider)
        {
            return provider.GetService<Bind<TBus, IEndpointDefinition<T>>>()?.Value;
        }

        public override void AddDefinition<T, TDefinition>()
        {
            Collection.AddSingleton<Bind<TBus, TDefinition>>();
            Collection.AddSingleton<Bind<TBus, T>>(provider => Bind<TBus>.Create<T>(ActivatorUtilities.CreateInstance<TDefinition>(provider)));
        }

        public override void AddEndpointDefinition<T, TDefinition>(IEndpointSettings<IEndpointDefinition<T>>? settings)
        {
            Collection.AddSingleton<TDefinition>();

            if (settings == null)
                Collection.AddSingleton(provider => Bind<TBus>.Create<IEndpointDefinition<T>>(provider.GetRequiredService<TDefinition>()));
            else
            {
                Collection.TryAddTransient<IEndpointSettings<IEndpointDefinition<T>>>(
                    _ => throw new InvalidOperationException("The settings are no longer configured in the container."));

                Collection.AddSingleton(provider =>
                    Bind<TBus>.Create<IEndpointDefinition<T>>(ActivatorUtilities.CreateInstance<TDefinition>(provider, settings)));
            }
        }

        public override void RegisterEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            Collection.TryAddSingleton(Bind<TBus>.Create(endpointNameFormatter));
        }

        public override void RegisterScopedClientFactory()
        {
            Collection.TryAddScoped(provider => Bind<TBus>.Create(GetScopedBusContext(provider)));
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
