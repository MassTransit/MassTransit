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
    public class ResponseEndpointConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IResponseEndpointConverter>> _types = new ConcurrentDictionary<Type, Lazy<IResponseEndpointConverter>>();

        IResponseEndpointConverter this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        public static Task Respond(ConsumeContext consumeContext, object message, Type messageType)
        {
            return Cached.Converters.Value[messageType].Respond(consumeContext, message);
        }

        public static Task Respond(ConsumeContext consumeContext, object message, Type messageType, IPipe<SendContext> pipe)
        {
            return Cached.Converters.Value[messageType].Respond(consumeContext, message, pipe);
        }

        static Lazy<IResponseEndpointConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<IResponseEndpointConverter>(() => CreateConverter(type));
        }

        static IResponseEndpointConverter CreateConverter(Type type)
        {
            var converterType = typeof(ResponseEndpointConverter<>).MakeGenericType(type);

            return (IResponseEndpointConverter)Activator.CreateInstance(converterType);
        }


        /// <summary>
        /// Calls the generic version of the ISendEndpoint.Send method with the object's type
        /// </summary>
        interface IResponseEndpointConverter
        {
            Task Respond(ConsumeContext consumeContext, object message);

            Task Respond(ConsumeContext consumeContext, object message, IPipe<SendContext> pipe);
        }


        /// <summary>
        /// Converts the object type message to the appropriate generic type and invokes the send method with that
        /// generic overload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class ResponseEndpointConverter<T> :
            IResponseEndpointConverter
            where T : class
        {
            Task IResponseEndpointConverter.Respond(ConsumeContext consumeContext, object message)
            {
                if (consumeContext == null)
                    throw new ArgumentNullException(nameof(consumeContext));
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                if (message is T msg)
                    return consumeContext.RespondAsync(msg);

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
            }

            Task IResponseEndpointConverter.Respond(ConsumeContext consumeContext, object message, IPipe<SendContext> pipe)
            {
                if (consumeContext == null)
                    throw new ArgumentNullException(nameof(consumeContext));
                if (message == null)
                    throw new ArgumentNullException(nameof(message));
                if (pipe == null)
                    throw new ArgumentNullException(nameof(pipe));

                if (message is T msg)
                    return consumeContext.RespondAsync(msg, pipe);

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
            }
        }


        static class Cached
        {
            internal static readonly Lazy<ResponseEndpointConverterCache> Converters =
                new Lazy<ResponseEndpointConverterCache>(() => new ResponseEndpointConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
