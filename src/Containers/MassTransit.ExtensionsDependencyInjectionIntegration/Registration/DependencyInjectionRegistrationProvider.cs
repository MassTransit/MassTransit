namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConfigurators;
    using Definition;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public class DependencyInjectionRegistrationProvider<TBus> :
        IRegistrationProvider
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionRegistrationProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IConsumerRegistration GetConsumerRegistration(Type consumerType)
        {
            var resolverType = typeof(TypedResolver<,>).MakeGenericType(typeof(TBus), consumerType);
            var resolver = (ITypedResolver)ActivatorUtilities.CreateInstance(_serviceProvider, resolverType);
            return resolver.Get();
        }

        public IConsumerRegistration<T> GetConsumerRegistration<T>()
            where T : class, IConsumer
        {
            return _serviceProvider.GetRequiredService<Bind<TBus, IConsumerRegistration<T>>>().Value;
        }

        public IEnumerable<IConsumerRegistration> GetConsumerRegistrations()
        {
            IEnumerable<Bind<TBus, IConsumerRegistration>> enumerable = _serviceProvider.GetServices<Bind<TBus, IConsumerRegistration>>();
            return enumerable.Select(x => x.Value);
        }
    }


    interface ITypedResolver
    {
        IConsumerRegistration Get();
    }


    public class TypedResolver<TBus, T> :
        ITypedResolver
        where T : class, IConsumer
    {
        readonly IServiceProvider _serviceProvider;

        public TypedResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IConsumerRegistration Get()
        {
            return _serviceProvider.GetRequiredService<Bind<TBus, IConsumerRegistration<T>>>().Value;
        }
    }


    public class DependencyInjectionComponentRegistrar<TBus> :
        IComponentRegistrar
    {
        readonly IServiceCollection _services;

        public DependencyInjectionComponentRegistrar(IServiceCollection services)
        {
            _services = services;
        }

        public void RegisterConsumer<T>(Type consumerDefinitionType, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            static IEnumerable<IConsumerConfiguratorAction<T>> GetActions(IServiceProvider serviceProvider)
            {
                return serviceProvider.GetService<IEnumerable<Bind<TBus, IConsumerConfiguratorAction<T>>>>()
                    .Select(x => x.Value);
            }

            if (consumerDefinitionType != null)
            {
                _services.TryAddSingleton(consumerDefinitionType);
                _services.TryAddSingleton(typeof(IConsumerDefinition<T>), consumerDefinitionType);
            }

            _services.TryAddScoped<T>();
            if (_services.All(d => d.ServiceType != typeof(Bind<TBus, IConsumerRegistration<T>>)))
            {
                _services.AddSingleton(provider => Bind<TBus>.Create((IConsumerRegistration<T>)new ConsumerRegistration<T>(() => GetActions(provider))));
                _services.AddSingleton(provider =>
                    Bind<TBus>.Create((IConsumerRegistration)provider.GetRequiredService<Bind<TBus, IConsumerRegistration<T>>>().Value));
            }

            _services.AddSingleton(provider => Bind<TBus>.Create(new ConsumerConfiguratorAction<T>(configure)));
        }


        class ConsumerConfiguratorAction<T> :
            IConsumerConfiguratorAction<T>
            where T : class, IConsumer
        {
            readonly Action<IConsumerConfigurator<T>> _configure;

            public ConsumerConfiguratorAction(Action<IConsumerConfigurator<T>> configure)
            {
                _configure = configure;
            }

            public void Configure(IConsumerConfigurator<T> configurator)
            {
                _configure?.Invoke(configurator);
            }
        }
    }
}
