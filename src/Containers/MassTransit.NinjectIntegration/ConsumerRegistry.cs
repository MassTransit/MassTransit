namespace MassTransit.NinjectIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;


    public class ConsumerRegistry :
        IConsumerRegistry
    {
        readonly ConcurrentDictionary<Type, ICachedConfigurator> _configurators = new ConcurrentDictionary<Type, ICachedConfigurator>();

        public ICachedConfigurator Add<T>()
            where T : class, IConsumer
        {
            return _configurators.GetOrAdd(typeof(T), _ => new ConsumerCachedConfigurator<T>());
        }

        public IEnumerable<ICachedConfigurator> GetConfigurators()
        {
            return _configurators.Values.ToList();
        }

        public ICachedConfigurator Add(Type consumerType)
        {
            return _configurators.GetOrAdd(consumerType,
                _ => (ICachedConfigurator)Activator.CreateInstance(typeof(ConsumerCachedConfigurator<>).MakeGenericType(consumerType)));
        }
    }
}
