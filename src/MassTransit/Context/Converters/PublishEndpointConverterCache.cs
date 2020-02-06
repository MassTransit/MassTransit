namespace MassTransit.Context.Converters
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Caches the converters that allow a raw object to be published using the object's type through
    /// the generic Send method.
    /// </summary>
    public class PublishEndpointConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IPublishEndpointConverter>> _types = new ConcurrentDictionary<Type, Lazy<IPublishEndpointConverter>>();

        public static Task Publish(IPublishEndpoint endpoint, object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].Publish(endpoint, message, cancellationToken);
        }

        public static Task Publish(IPublishEndpoint endpoint, object message, Type messageType, IPipe<PublishContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].Publish(endpoint, message, pipe, cancellationToken);
        }

        IPublishEndpointConverter this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        static Lazy<IPublishEndpointConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<IPublishEndpointConverter>(() => CreateConverter(type));
        }

        static IPublishEndpointConverter CreateConverter(Type type)
        {
            Type converterType = typeof(PublishEndpointConverter<>).MakeGenericType(type);

            return (IPublishEndpointConverter)Activator.CreateInstance(converterType);
        }


        static class Cached
        {
            internal static readonly Lazy<PublishEndpointConverterCache> Converters =
                new Lazy<PublishEndpointConverterCache>(() => new PublishEndpointConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
