namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// A content filter applies a delegate to the message context, and uses the result to either accept the message
    /// or discard it.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class ContextFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly Func<TContext, Task<bool>> _filter;

        public ContextFilter(Func<TContext, Task<bool>> filter)
        {
            _filter = filter;
        }

        public Task Send(TContext context, IPipe<TContext> next)
        {
            Task<bool> filterTask = _filter(context);
            if (filterTask.Status == TaskStatus.RanToCompletion && filterTask.Result)
                return next.Send(context);

            async Task SendAsync()
            {
                var accept = await filterTask.ConfigureAwait(false);
                if (accept)
                    await next.Send(context).ConfigureAwait(false);
            }

            return SendAsync();
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("context");
        }
    }
}
