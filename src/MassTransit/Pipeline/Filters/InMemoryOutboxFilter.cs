namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public class InMemoryOutboxFilter<TContext, TResult> :
        IFilter<TContext>
        where TContext : class, ConsumeContext
        where TResult : TContext, OutboxContext
    {
        readonly Func<TContext, TResult> _contextFactory;

        public InMemoryOutboxFilter(Func<TContext, TResult> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            var outboxContext = _contextFactory(context);

            try
            {
                await next.Send(outboxContext).ConfigureAwait(false);

                await outboxContext.ExecutePendingActions().ConfigureAwait(false);

                await outboxContext.ConsumeCompleted.ConfigureAwait(false);
            }
            catch (Exception)
            {
                await outboxContext.DiscardPendingActions().ConfigureAwait(false);

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("outbox");
            scope.Add("type", "in-memory");
        }
    }
}
