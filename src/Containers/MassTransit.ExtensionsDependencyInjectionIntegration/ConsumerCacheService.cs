namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class ConsumerCacheService : IConsumerCacheService
    {
        private static ConcurrentDictionary<Type, ICachedConfigurator> _consumerHandlers = new ConcurrentDictionary<Type, ICachedConfigurator>();

        public void Add<T>()
            where T : class, IConsumer
        {
            _consumerHandlers.GetOrAdd(typeof(T), _ => new CachedEndPointConsumerConfigurator<T>());
        }

        public IEnumerable<ICachedConfigurator> GetHandlers()
        {
            return _consumerHandlers.Values.ToList();
        }

        public ConcurrentDictionary<Type, ICachedConfigurator> Instance
        {
            get
            {
                return _consumerHandlers;
            }
        }

        public void Configure(Type consumerType, IReceiveEndpointConfigurator configurator, IServiceProvider container)
        {
            _consumerHandlers
                .GetOrAdd(consumerType, _ => (ICachedConfigurator)Activator.CreateInstance(typeof(ICachedConfigurator).MakeGenericType(consumerType)))
                .Configure(configurator, container);
        }
    }

    public class CachedEndPointConsumerConfigurator<T> : ICachedConfigurator
        where T : class, IConsumer
    {
        public void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider services)
        {
            configurator.Consumer(new ExtensionsDependencyInjectionConsumerFactory<T>(services));
        }
    }

    public interface IConsumerCacheService
    {
        void Add<T>() where T : class, IConsumer;
        IEnumerable<ICachedConfigurator> GetHandlers();
        ConcurrentDictionary<Type, ICachedConfigurator> Instance { get; }
        void Configure(Type consumerType, IReceiveEndpointConfigurator configurator, IServiceProvider container);
    }
}
