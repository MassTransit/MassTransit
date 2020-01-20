namespace MassTransit
{
    using NHibernate;
    using Saga;
    using NHibernateIntegration.Saga;


    public static class NHibernateSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a Redis saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> NHibernateRepository<T>(this ISagaRegistrationConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.Repository(x =>
                x.RegisterSagaRepository<T, ISession, NHibernateSagaConsumeContextFactory<T>, NHibernateSagaRepositoryContextFactory<T>>());

            return configurator;
        }
    }
}
