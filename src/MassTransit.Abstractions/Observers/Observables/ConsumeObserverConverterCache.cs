namespace MassTransit.Observables
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;


    /// <summary>
    /// Caches the converters that allow a raw object to be published using the object's type through
    /// the generic Send method.
    /// </summary>
    public class ConsumeObserverConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IConsumeObserverConverter>> _types = new ConcurrentDictionary<Type, Lazy<IConsumeObserverConverter>>();

        IConsumeObserverConverter this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        public static Task PreConsume(Type messageType, IConsumeObserver observer, object context)
        {
            return Cached.Converters.Value[messageType].PreConsume(observer, context);
        }

        public static Task PostConsume(Type messageType, IConsumeObserver observer, object context)
        {
            return Cached.Converters.Value[messageType].PostConsume(observer, context);
        }

        public static Task ConsumeFault(Type messageType, IConsumeObserver observer, object context, Exception exception)
        {
            return Cached.Converters.Value[messageType].ConsumeFault(observer, context, exception);
        }

        static Lazy<IConsumeObserverConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<IConsumeObserverConverter>(() => CreateConverter(type));
        }

        static IConsumeObserverConverter CreateConverter(Type type)
        {
            if (type.ClosesType(typeof(ConsumeContext<>)))
            {
                var messageType = type.GetClosingArguments(typeof(ConsumeContext<>)).Single();

                var converterType = typeof(ConsumeObserverConverter<>).MakeGenericType(messageType);

                return (IConsumeObserverConverter)Activator.CreateInstance(converterType);
            }

            throw new ArgumentException($"The context was not a ConsumeContext: {TypeCache.GetShortName(type)}", nameof(type));
        }


        static class Cached
        {
            internal static readonly Lazy<ConsumeObserverConverterCache> Converters =
                new Lazy<ConsumeObserverConverterCache>(() => new ConsumeObserverConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
