namespace MassTransit.Registration
{
    using Saga;


    public class SagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
        }
    }
}
