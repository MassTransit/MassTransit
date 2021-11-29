namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Consumer;


    public class ConsumerConnectorCache<TConsumer> :
        IConsumerConnectorCache
        where TConsumer : class
    {
        readonly Lazy<ConsumerConnector<TConsumer>> _connector;

        ConsumerConnectorCache()
        {
            _connector = new Lazy<ConsumerConnector<TConsumer>>(() => new ConsumerConnector<TConsumer>(),
                LazyThreadSafetyMode.PublicationOnly);
        }

        public static IConsumerConnector Connector => Cached.Instance.Value.Connector;

        IConsumerConnector IConsumerConnectorCache.Connector => _connector.Value;


        static class Cached
        {
            internal static readonly Lazy<IConsumerConnectorCache> Instance = new Lazy<IConsumerConnectorCache>(
                () => new ConsumerConnectorCache<TConsumer>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }


    public static class ConsumerConnectorCache
    {
        static CachedConnector GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                (CachedConnector)Activator.CreateInstance(typeof(CachedConnector<>).MakeGenericType(type)));
        }

        public static ConnectHandle Connect(IConsumePipeConnector consumePipe, Type consumerType, Func<Type, object> objectFactory)
        {
            return GetOrAdd(consumerType).Connect(consumePipe, objectFactory);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConnector> Instance =
                new ConcurrentDictionary<Type, CachedConnector>();
        }


        interface CachedConnector
        {
            ConnectHandle Connect(IConsumePipeConnector consumePipe, Func<Type, object> objectFactory);
        }


        class CachedConnector<T> :
            CachedConnector
            where T : class
        {
            readonly Lazy<IConsumerConnector> _connector;

            public CachedConnector()
            {
                _connector = new Lazy<IConsumerConnector>(() => ConsumerConnectorCache<T>.Connector);
            }

            public ConnectHandle Connect(IConsumePipeConnector consumePipe, Func<Type, object> objectFactory)
            {
                var consumerFactory = new ObjectConsumerFactory<T>(objectFactory);

                IConsumerSpecification<T> specification = _connector.Value.CreateConsumerSpecification<T>();

                return _connector.Value.ConnectConsumer(consumePipe, consumerFactory, specification);
            }
        }
    }
}
