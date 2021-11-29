namespace MassTransit.Saga
{
    using System;
    using System.Threading.Tasks;


    public class InMemorySagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<IndexedSagaDictionary<TSaga>, TSaga>
        where TSaga : class, ISaga
    {
        public async Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(IndexedSagaDictionary<TSaga> context, ConsumeContext<T> consumeContext,
            TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            SagaInstance<TSaga> sagaInstance;
            switch (mode)
            {
                case SagaConsumeContextMode.Add:
                case SagaConsumeContextMode.Insert:
                    sagaInstance = new SagaInstance<TSaga>(instance);

                    await sagaInstance.MarkInUse(consumeContext.CancellationToken).ConfigureAwait(false);

                    context.Add(sagaInstance);
                    break;

                case SagaConsumeContextMode.Load:
                    sagaInstance = context[instance.CorrelationId];

                    await sagaInstance.MarkInUse(consumeContext.CancellationToken).ConfigureAwait(false);

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            return new InMemorySagaConsumeContext<TSaga, T>(consumeContext, sagaInstance);
        }
    }
}
