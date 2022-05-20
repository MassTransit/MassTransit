namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Logging;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TSaga">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class OrchestratesSagaMessageFilter<TSaga, TMessage> :
        ISagaMessageFilter<TSaga, TMessage>
        where TSaga : class, ISaga, Orchestrates<TMessage>
        where TMessage : class, CorrelatedBy<Guid>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("orchestrates");
            scope.Add("method", $"Consume({TypeCache<TMessage>.ShortName} message)");
        }

        public async Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            StartedActivity? activity = LogContext.Current?.StartSagaActivity(context);
            try
            {
                await context.Saga.Consume(context).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
