namespace MassTransit.Context
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Caches the converters that allow a raw object to be published using the object's type through
    /// the generic Send method.
    /// </summary>
    public class PublishEndpointConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IPublishEndpointConverter>> _types = new ConcurrentDictionary<Type, Lazy<IPublishEndpointConverter>>();

        IPublishEndpointConverter this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        public static Task Publish(IPublishEndpoint endpoint, object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].Publish(endpoint, message, cancellationToken);
        }

        public static Task Publish(IPublishEndpoint endpoint, object message, Type messageType, IPipe<PublishContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].Publish(endpoint, message, pipe, cancellationToken);
        }

        public static Task PublishInitializer(IPublishEndpoint endpoint, Type messageType, object values, CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].PublishInitializer(endpoint, values, cancellationToken);
        }

        public static Task PublishInitializer(IPublishEndpoint endpoint, Type messageType, object values, IPipe<PublishContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].PublishInitializer(endpoint, values, pipe, cancellationToken);
        }

        static Lazy<IPublishEndpointConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<IPublishEndpointConverter>(() => CreateConverter(type));
        }

        static IPublishEndpointConverter CreateConverter(Type type)
        {
            return Activator.CreateInstance(typeof(PublishEndpointConverter<>).MakeGenericType(type)) as IPublishEndpointConverter
                ?? throw new InvalidOperationException("Failed to create PublishEndpointConverter");
        }


        /// <summary>
        /// Calls the generic version of the IPublishEndpoint.Send method with the object's type
        /// </summary>
        public interface IPublishEndpointConverter
        {
            Task Publish(IPublishEndpoint endpoint, object message, CancellationToken cancellationToken = default);

            Task Publish(IPublishEndpoint endpoint, object message, IPipe<PublishContext> pipe, CancellationToken cancellationToken = default);

            Task PublishInitializer(IPublishEndpoint endpoint, object values, CancellationToken cancellationToken = default);

            Task PublishInitializer(IPublishEndpoint endpoint, object values, IPipe<PublishContext> pipe, CancellationToken cancellationToken = default);
        }


        /// <summary>
        /// Converts the object message type to the generic type T and publishes it on the endpoint specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class PublishEndpointConverter<T> :
            IPublishEndpointConverter
            where T : class
        {
            public Task Publish(IPublishEndpoint endpoint, object message, CancellationToken cancellationToken)
            {
                if (endpoint == null)
                    throw new ArgumentNullException(nameof(endpoint));
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                if (message is T msg)
                    return endpoint.Publish(msg, cancellationToken);

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
            }

            public Task Publish(IPublishEndpoint endpoint, object message, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
            {
                if (endpoint == null)
                    throw new ArgumentNullException(nameof(endpoint));
                if (message == null)
                    throw new ArgumentNullException(nameof(message));
                if (pipe == null)
                    throw new ArgumentNullException(nameof(pipe));

                if (message is T msg)
                    return endpoint.Publish(msg, pipe, cancellationToken);

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
            }

            public Task PublishInitializer(IPublishEndpoint endpoint, object values, CancellationToken cancellationToken)
            {
                if (endpoint == null)
                    throw new ArgumentNullException(nameof(endpoint));
                if (values == null)
                    throw new ArgumentNullException(nameof(values));

                return endpoint.Publish<T>(values, cancellationToken);
            }

            public Task PublishInitializer(IPublishEndpoint endpoint, object values, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
            {
                if (endpoint == null)
                    throw new ArgumentNullException(nameof(endpoint));
                if (values == null)
                    throw new ArgumentNullException(nameof(values));
                if (pipe == null)
                    throw new ArgumentNullException(nameof(pipe));

                return endpoint.Publish<T>(values, pipe, cancellationToken);
            }
        }


        static class Cached
        {
            internal static readonly Lazy<PublishEndpointConverterCache> Converters =
                new Lazy<PublishEndpointConverterCache>(() => new PublishEndpointConverterCache());
        }
    }
}
