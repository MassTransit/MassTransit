namespace MassTransit.NinjectIntegration
{
    using Ninject;
    using Saga;


    public class SagaCachedConfigurator<T> :
        ICachedConfigurator
        where T : class, ISaga
    {
        public void Configure(IReceiveEndpointConfigurator configurator, IKernel kernel)
        {
            var sagaRepository = kernel.Get<ISagaRepository<T>>();

            var repository = new NinjectSagaRepository<T>(sagaRepository);

            configurator.Saga(repository);
        }
    }
}
