namespace MassTransit.Observables
{
    using System.Threading.Tasks;
    using Util;


    public class RetryObservable :
        Connectable<IRetryObserver>,
        IRetryObserver
    {
        public Task PostCreate<T>(RetryPolicyContext<T> context)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.PostCreate(context));
        }

        public Task PostFault<T>(RetryContext<T> context)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.PostFault(context));
        }

        public Task PreRetry<T>(RetryContext<T> context)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.PreRetry(context));
        }

        public Task RetryFault<T>(RetryContext<T> context)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.RetryFault(context));
        }

        public Task RetryComplete<T>(RetryContext<T> context)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.RetryComplete(context));
        }

        public Task RetryFault(RetryContext context)
        {
            return ForEachAsync(x => RetryFaultObserverCache.RetryFault(x, context, context.ContextType));
        }
    }
}
