namespace MassTransit.Observables
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Converts the object message type to the generic type T and publishes it on the endpoint specified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsumeObserverConverter<T> :
        IConsumeObserverConverter
        where T : class
    {
        Task IConsumeObserverConverter.PreConsume(IConsumeObserver observer, object context)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var consumeContext = context as ConsumeContext<T>;
            if (consumeContext == null)
                throw new ArgumentException("Unexpected context type: " + TypeCache.GetShortName(context.GetType()));

            return observer.PreConsume(consumeContext);
        }

        Task IConsumeObserverConverter.PostConsume(IConsumeObserver observer, object context)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var consumeContext = context as ConsumeContext<T>;
            if (consumeContext == null)
                throw new ArgumentException("Unexpected context type: " + TypeCache.GetShortName(context.GetType()));

            return observer.PostConsume(consumeContext);
        }

        Task IConsumeObserverConverter.ConsumeFault(IConsumeObserver observer, object context, Exception exception)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var consumeContext = context as ConsumeContext<T>;
            if (consumeContext == null)
                throw new ArgumentException("Unexpected context type: " + TypeCache.GetShortName(context.GetType()));

            return observer.ConsumeFault(consumeContext, exception);
        }
    }
}
