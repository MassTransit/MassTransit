namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public class InMemoryOutboxFilter :
        IFilter<ConsumeContext>
    {
        public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            InMemoryOutboxMessageSchedulerContext outboxSchedulerContext = null;
            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                outboxSchedulerContext = new InMemoryOutboxMessageSchedulerContext(schedulerContext);
                context.AddOrUpdatePayload(() => outboxSchedulerContext, _ => outboxSchedulerContext);
            }

            var outboxContext = new InMemoryOutboxConsumeContext(context);

            try
            {
                await next.Send(outboxContext).ConfigureAwait(false);

                await outboxContext.ExecutePendingActions().ConfigureAwait(false);

                await outboxContext.ConsumeCompleted.ConfigureAwait(false);
            }
            catch (Exception)
            {
                await outboxContext.DiscardPendingActions().ConfigureAwait(false);

                try
                {
                    if (outboxSchedulerContext != null)
                        await outboxSchedulerContext.CancelAllScheduledMessages().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    LogContext.Warning?.Log(e, "One or more messages could not be unscheduled.", e);
                }

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("outbox");
            scope.Add("type", "in-memory");
        }
    }


    public class InMemoryOutboxFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            InMemoryOutboxMessageSchedulerContext outboxSchedulerContext = null;
            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                outboxSchedulerContext = new InMemoryOutboxMessageSchedulerContext(schedulerContext);
                context.AddOrUpdatePayload(() => outboxSchedulerContext, _ => outboxSchedulerContext);
            }

            var outboxContext = new InMemoryOutboxConsumeContext<T>(context);
            try
            {
                await next.Send(outboxContext).ConfigureAwait(false);

                await outboxContext.ExecutePendingActions().ConfigureAwait(false);

                await outboxContext.ConsumeCompleted.ConfigureAwait(false);
            }
            catch (Exception)
            {
                await outboxContext.DiscardPendingActions().ConfigureAwait(false);

                try
                {
                    if (outboxSchedulerContext != null)
                        await outboxSchedulerContext.CancelAllScheduledMessages().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    LogContext.Warning?.Log(e, "One or more messages could not be unscheduled.", e);
                }

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
