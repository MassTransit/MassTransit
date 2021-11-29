namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;


    public class DelegateFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly Action<TContext> _callback;

        public DelegateFilter(Action<TContext> callback)
        {
            _callback = callback;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("delegate");
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public Task Send(TContext context, IPipe<TContext> next)
        {
            _callback(context);

            return next.Send(context);
        }
    }
}
