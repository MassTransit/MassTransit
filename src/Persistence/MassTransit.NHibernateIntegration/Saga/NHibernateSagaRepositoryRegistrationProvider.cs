namespace MassTransit.NHibernateIntegration.Saga
{
    using MassTransit.Saga;
    using Registration;


    public class NHibernateSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            configurator.NHibernateRepository();
        }
    }
}
