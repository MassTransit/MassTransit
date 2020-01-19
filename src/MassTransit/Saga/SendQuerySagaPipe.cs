namespace MassTransit.Saga
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;


    public class SendQuerySagaPipe<TSaga, T> :
        IPipe<SagaRepositoryQueryContext<TSaga, T>>
        where TSaga : class, ISaga
        where T : class
    {
        readonly ISagaPolicy<TSaga, T> _policy;
        readonly IPipe<SagaConsumeContext<TSaga, T>> _next;

        public SendQuerySagaPipe(ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            _policy = policy;
            _next = next;
        }

        public void Probe(ProbeContext context)
        {
        }

        public async Task Send(SagaRepositoryQueryContext<TSaga, T> context)
        {
            if (context.Count > 0)
            {
                async Task LoadInstance(Guid correlationId)
                {
                    SagaConsumeContext<TSaga, T> sagaConsumeContext = await context.Load(correlationId).ConfigureAwait(false);
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

                await Task.WhenAll(context.Select(LoadInstance)).ConfigureAwait(false);
            }
            else
            {
                var missingPipe = new MissingSagaPipe<TSaga, T>(context, _next);

                await _policy.Missing(context, missingPipe).ConfigureAwait(false);
            }
        }
    }
}
