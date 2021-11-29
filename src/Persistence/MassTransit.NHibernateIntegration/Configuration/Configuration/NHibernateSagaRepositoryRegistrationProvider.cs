namespace MassTransit.Configuration
{
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
