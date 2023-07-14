namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;


    public class PayloadFilter<TContext, TPayload> :
        IFilter<TContext>
        where TContext : class, PipeContext
        where TPayload : class
    {
        readonly TPayload _payload;

        public PayloadFilter(TPayload payload)
        {
            _payload = payload;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("inline");
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public Task Send(TContext context, IPipe<TContext> next)
        {
            context.GetOrAddPayload(() => _payload);

            return next.Send(context);
        }
    }
}
