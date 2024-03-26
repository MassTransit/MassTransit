namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using InMemoryOutbox;
    using Microsoft.Extensions.DependencyInjection;


    public class InMemoryOutboxFilter<TContext, TResult> :
        IFilter<TContext>
        where TContext : class, ConsumeContext
        where TResult : TContext, OutboxContext
    {
        readonly bool _concurrentMessageDelivery;
        readonly Func<TContext, TResult> _contextFactory;
        readonly ISetScopedConsumeContext _setter;

        public InMemoryOutboxFilter(ISetScopedConsumeContext setter, Func<TContext, TResult> contextFactory, bool concurrentMessageDelivery)
        {
            _setter = setter;
            _contextFactory = contextFactory;
            _concurrentMessageDelivery = concurrentMessageDelivery;
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            var outboxContext = _contextFactory(context);

            IDisposable pop = null;
            if (context.TryGetPayload(out IServiceScope scope))
                pop = _setter.PushContext(scope, outboxContext);

            try
            {
                await next.Send(outboxContext).ConfigureAwait(false);

                await outboxContext.ExecutePendingActions(_concurrentMessageDelivery).ConfigureAwait(false);

                await outboxContext.ConsumeCompleted.ConfigureAwait(false);
            }
            catch (Exception)
            {
                await outboxContext.DiscardPendingActions().ConfigureAwait(false);

                throw;
            }
            finally
            {
                pop?.Dispose();
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("outbox");
            scope.Add("type", "in-memory");
        }

        public void Method1()
        {
        }

        public void Method2()
        {
        }

        public void Method3()
        {
        }
    }
}
