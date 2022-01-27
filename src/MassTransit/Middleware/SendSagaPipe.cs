namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using Saga;


    public class SendSagaPipe<TSaga, T> :
        IPipe<SagaRepositoryContext<TSaga, T>>
        where TSaga : class, ISaga
        where T : class
    {
        readonly Guid _correlationId;
        readonly IPipe<SagaConsumeContext<TSaga, T>> _next;
        readonly ISagaPolicy<TSaga, T> _policy;

        public SendSagaPipe(ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next, Guid correlationId)
        {
            _policy = policy;
            _next = next;
            _correlationId = correlationId;
        }

        public void Probe(ProbeContext context)
        {
        }

        public async Task Send(SagaRepositoryContext<TSaga, T> context)
        {
            SagaConsumeContext<TSaga, T> sagaConsumeContext = null;

            if (_policy.PreInsertInstance(context, out var instance))
                sagaConsumeContext = await context.Insert(instance).ConfigureAwait(false);

            sagaConsumeContext ??= await context.Load(_correlationId).ConfigureAwait(false);
            if (sagaConsumeContext != null)
            {
                try
                {
                    sagaConsumeContext.LogUsed();

                    await _policy.Existing(sagaConsumeContext, _next).ConfigureAwait(false);

                    if (_policy.IsReadOnly)
                        await context.Undo(sagaConsumeContext).ConfigureAwait(false);
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
            else
                await _policy.Missing(context, new MissingSagaPipe<TSaga, T>(context, _next)).ConfigureAwait(false);
        }
    }
}
