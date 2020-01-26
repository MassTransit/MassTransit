namespace MassTransit.MartenIntegration.Saga.Context
{
    using System.Threading.Tasks;
    using Marten;
    using MassTransit.Saga;


    public class MartenSagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<IDocumentSession, TSaga>
        where TSaga : class, ISaga
    {
        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(IDocumentSession context, ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            var sagaConsumeContext = new MartenSagaConsumeContext<TSaga, T>(context, consumeContext, instance, mode);

            return Task.FromResult<SagaConsumeContext<TSaga, T>>(sagaConsumeContext);
        }
    }
}
