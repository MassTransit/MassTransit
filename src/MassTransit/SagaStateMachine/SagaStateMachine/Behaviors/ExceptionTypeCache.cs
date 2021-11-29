namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;


    public static class ExceptionTypeCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ => (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static Task Faulted<TSaga>(IBehavior<TSaga> behavior, BehaviorContext<TSaga> context, Exception exception)
            where TSaga : class, ISaga
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return GetOrAdd(exception.GetType()).Faulted(behavior, context, exception);
        }

        public static Task Faulted<TSaga, TMessage>(IBehavior<TSaga, TMessage> behavior, BehaviorContext<TSaga, TMessage> context, Exception exception)
            where TSaga : class, ISaga
            where TMessage : class
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return GetOrAdd(exception.GetType()).Faulted(behavior, context, exception);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance = new ConcurrentDictionary<Type, CachedConfigurator>();
        }


        interface CachedConfigurator
        {
            Task Faulted<TSaga>(IBehavior<TSaga> behavior, BehaviorContext<TSaga> context, Exception exception)
                where TSaga : class, ISaga;

            Task Faulted<TSaga, TMessage>(IBehavior<TSaga, TMessage> behavior, BehaviorContext<TSaga, TMessage> context, Exception exception)
                where TSaga : class, ISaga
                where TMessage : class;
        }


        class CachedConfigurator<TException> :
            CachedConfigurator
            where TException : Exception
        {
            Task CachedConfigurator.Faulted<TInstance>(IBehavior<TInstance> behavior, BehaviorContext<TInstance> context, Exception exception)
            {
                if (exception is TException typedException)
                {
                    var exceptionContext = new BehaviorExceptionContextProxy<TInstance, TException>(context, typedException);

                    return behavior.Faulted(exceptionContext);
                }

                throw new ArgumentException($"The exception type {exception.GetType().Name} did not match the expected type {typeof(TException).Name}");
            }

            Task CachedConfigurator.Faulted<TInstance, TData>(IBehavior<TInstance, TData> behavior, BehaviorContext<TInstance, TData> context,
                Exception exception)
            {
                if (exception is TException typedException)
                {
                    var exceptionContext = new BehaviorExceptionContextProxy<TInstance, TData, TException>(context, typedException);

                    return behavior.Faulted(exceptionContext);
                }

                throw new ArgumentException($"The exception type {exception.GetType().Name} did not match the expected type {typeof(TException).Name}");
            }
        }
    }
}
