namespace MassTransit.NHibernateIntegration.Saga
{
    using System.Threading.Tasks;
    using MassTransit.Saga;


    public class NHibernateSagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<NHibernateContext, TSaga>
        where TSaga : class, ISaga
    {
        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(NHibernateContext context, ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            return Task.FromResult<SagaConsumeContext<TSaga, T>>(new NHibernateSagaConsumeContext<TSaga, T>(context.Session, consumeContext, instance, mode));
        }
    }
}
