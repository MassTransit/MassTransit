namespace MassTransit.Observables
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class FilterObservable :
        Connectable<IFilterObserver>,
        IFilterObserver
    {
        public Task PreSend<T>(T context)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.PreSend(context));
        }

        public Task PostSend<T>(T context)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.PostSend(context));
        }

        public Task SendFault<T>(T context, Exception exception)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.SendFault(context, exception));
        }
    }


    public class FilterObservable<TContext> :
        Connectable<IFilterObserver<TContext>>,
        IFilterObserver<TContext>
        where TContext : class, PipeContext
    {
        public Task PreSend(TContext context)
        {
            return ForEachAsync(x => x.PreSend(context));
        }

        public Task PostSend(TContext context)
        {
            return ForEachAsync(x => x.PostSend(context));
        }

        public Task SendFault(TContext context, Exception exception)
        {
            return ForEachAsync(x => x.SendFault(context, exception));
        }
    }
}
