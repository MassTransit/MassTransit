namespace MassTransit.Middleware
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using Saga;


    public class SendQuerySagaPipe<TSaga, T> :
        IPipe<SagaRepositoryQueryContext<TSaga, T>>
        where TSaga : class, ISaga
        where T : class
    {
        readonly IPipe<SagaConsumeContext<TSaga, T>> _next;
        readonly ISagaPolicy<TSaga, T> _policy;

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

                            if (_policy.IsReadOnly)
                            {
                                await context.Undo(sagaConsumeContext).ConfigureAwait(false);
                            }
                            else
                            {
                                if (sagaConsumeContext.IsCompleted)
                                {
                                    await context.Delete(sagaConsumeContext).ConfigureAwait(false);

                                    sagaConsumeContext.LogRemoved();
                                }
                                else
                                    await context.Update(sagaConsumeContext).ConfigureAwait(false);
                            }
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
