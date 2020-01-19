namespace MassTransit.NHibernateIntegration.Saga
{
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NHibernate;


    public class NHibernateSagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<ISession, TSaga>
        where TSaga : class, ISaga
    {
        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ISession context, ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            return Task.FromResult<SagaConsumeContext<TSaga, T>>(new NHibernateSagaConsumeContext<TSaga, T>(context, consumeContext, instance, mode));
        }
    }
}
