namespace MassTransit.Observables
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;


    public class RetryFaultObserverCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IRetryFaultObserver>> _types = new ConcurrentDictionary<Type, Lazy<IRetryFaultObserver>>();

        IRetryFaultObserver this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        public static Task RetryFault(IRetryObserver observer, RetryContext context, Type contextType)
        {
            return Cached.Converters.Value[contextType].RetryFault(observer, context);
        }

        static Lazy<IRetryFaultObserver> CreateTypeConverter(Type type)
        {
            return new Lazy<IRetryFaultObserver>(() => CreateConverter(type));
        }

        static IRetryFaultObserver CreateConverter(Type type)
        {
            var converterType = typeof(RetryFaultObserver<>).MakeGenericType(type);

            return (IRetryFaultObserver)Activator.CreateInstance(converterType);
        }


        interface IRetryFaultObserver
        {
            Task RetryFault(IRetryObserver observer, RetryContext context);
        }


        class RetryFaultObserver<T> :
            IRetryFaultObserver
            where T : class, PipeContext
        {
            public Task RetryFault(IRetryObserver observer, RetryContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                return observer.RetryFault((RetryContext<T>)context);
            }
        }


        static class Cached
        {
            internal static readonly Lazy<RetryFaultObserverCache> Converters =
                new Lazy<RetryFaultObserverCache>(() => new RetryFaultObserverCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
