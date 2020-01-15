namespace MassTransit.Saga
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;


    public class SendQuerySagaPipe<TSaga, T> :
        IPipe<SagaRepositoryContext<TSaga, T>>
        where TSaga : class, ISaga
        where T : class
    {
        readonly ISagaQuery<TSaga> _query;
        readonly ISagaPolicy<TSaga, T> _policy;
        readonly IPipe<SagaConsumeContext<TSaga, T>> _next;

        public SendQuerySagaPipe(ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            _query = query;
            _policy = policy;
            _next = next;
        }

        public void Probe(ProbeContext context)
        {
        }

        public async Task Send(SagaRepositoryContext<TSaga, T> context)
        {
            SagaRepositoryQueryContext<TSaga, T> queryContext = await context.Query(_query).ConfigureAwait(false);
            if (queryContext.Count > 0)
            {
                async Task LoadInstance(Guid correlationId)
                {
                    SagaConsumeContext<TSaga, T> sagaConsumeContext = await queryContext.Load(correlationId).ConfigureAwait(false);
                    if (sagaConsumeContext != null)
                    {
                        sagaConsumeContext.LogUsed();

                        try
                        {
                            await _policy.Existing(sagaConsumeContext, _next).ConfigureAwait(false);
                        }
                        finally
                        {
                            switch (sagaConsumeContext)
                            {
                                case IAsyncDisposable asyncDisposable:
                                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                                    break;
                                case IDisposable disposable:
                                    disposable.Dispose();
                                    break;
                            }
                        }
                    }
                }

                await Task.WhenAll(queryContext.Select(LoadInstance)).ConfigureAwait(false);
            }
            else
            {
                var missingPipe = new MissingSagaPipe<TSaga, T>(queryContext, _next);

                await _policy.Missing(context, missingPipe).ConfigureAwait(false);
            }
        }
    }
}
