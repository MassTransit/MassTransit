namespace MassTransit.Context.Converters
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;

#nullable enable


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


        static class Cached
        {
            internal static readonly Lazy<ResponseEndpointConverterCache> Converters =
                new Lazy<ResponseEndpointConverterCache>(() => new ResponseEndpointConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
