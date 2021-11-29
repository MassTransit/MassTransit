namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;


    public class AsyncDelegateFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly Func<TContext, Task> _callback;

        public AsyncDelegateFilter(Func<TContext, Task> callback)
        {
            _callback = callback;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("asyncDelegate");
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public Task Send(TContext context, IPipe<TContext> next)
        {
            var callbackTask = _callback(context);
            if (callbackTask.Status == TaskStatus.RanToCompletion)
                return next.Send(context);

            async Task SendAsync()
            {
                await callbackTask.ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }

            return SendAsync();
        }
    }
}
