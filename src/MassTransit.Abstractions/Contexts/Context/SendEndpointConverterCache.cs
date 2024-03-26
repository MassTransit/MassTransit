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
    public class SendEndpointConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<ISendEndpointConverter>> _types = new ConcurrentDictionary<Type, Lazy<ISendEndpointConverter>>();

        ISendEndpointConverter this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        public static Task Send(ISendEndpoint endpoint, object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].Send(endpoint, message, cancellationToken);
        }

        public static Task Send(ISendEndpoint endpoint, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].Send(endpoint, message, pipe, cancellationToken);
        }

        public static Task SendInitializer(ISendEndpoint endpoint, Type messageType, object values, CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].SendInitializer(endpoint, values, cancellationToken);
        }

        public static Task SendInitializer(ISendEndpoint endpoint, Type messageType, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].SendInitializer(endpoint, values, pipe, cancellationToken);
        }

        static Lazy<ISendEndpointConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<ISendEndpointConverter>(() => CreateConverter(type));
        }

        static ISendEndpointConverter CreateConverter(Type type)
        {
            return Activator.CreateInstance(typeof(SendEndpointConverter<>).MakeGenericType(type)) as ISendEndpointConverter
                ?? throw new InvalidOperationException("Failed to create SendEndpointConverter");
        }


        /// <summary>
        /// Calls the generic version of the ISendEndpoint.Send method with the object's type
        /// </summary>
        interface ISendEndpointConverter
        {
            Task Send(ISendEndpoint endpoint, object message, CancellationToken cancellationToken = default);

            Task Send(ISendEndpoint endpoint, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default);

            Task SendInitializer(ISendEndpoint endpoint, object values, CancellationToken cancellationToken = default);

            Task SendInitializer(ISendEndpoint endpoint, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = default);
        }


        /// <summary>
        /// Converts the object type message to the appropriate generic type and invokes the send method with that
        /// generic overload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class SendEndpointConverter<T> :
            ISendEndpointConverter
            where T : class
        {
            public Task Send(ISendEndpoint endpoint, object message, CancellationToken cancellationToken)
            {
                if (endpoint == null)
                    throw new ArgumentNullException(nameof(endpoint));
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                if (message is T msg)
                    return endpoint.Send(msg, cancellationToken);

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
            }

            public Task Send(ISendEndpoint endpoint, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            {
                if (endpoint == null)
                    throw new ArgumentNullException(nameof(endpoint));
                if (message == null)
                    throw new ArgumentNullException(nameof(message));
                if (pipe == null)
                    throw new ArgumentNullException(nameof(pipe));

                if (message is T msg)
                    return endpoint.Send(msg, pipe, cancellationToken);

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
            }

            public Task SendInitializer(ISendEndpoint endpoint, object values, CancellationToken cancellationToken)
            {
                if (endpoint == null)
                    throw new ArgumentNullException(nameof(endpoint));
                if (values == null)
                    throw new ArgumentNullException(nameof(values));

                return endpoint.Send<T>(values, cancellationToken);
            }

            public Task SendInitializer(ISendEndpoint endpoint, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            {
                if (endpoint == null)
                    throw new ArgumentNullException(nameof(endpoint));
                if (values == null)
                    throw new ArgumentNullException(nameof(values));
                if (pipe == null)
                    throw new ArgumentNullException(nameof(pipe));

                return endpoint.Send<T>(values, pipe, cancellationToken);
            }
        }


        static class Cached
        {
            internal static readonly Lazy<SendEndpointConverterCache> Converters = new Lazy<SendEndpointConverterCache>(() => new SendEndpointConverterCache());
        }
    }
}
