namespace MassTransit
{
    using Configuration;
    using NHibernate;
    using NHibernateIntegration.Saga;
    using Saga;


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
                x.RegisterSagaRepository<T, ISession, SagaConsumeContextFactory<ISession, T>, NHibernateSagaRepositoryContextFactory<T>>());

            return configurator;
        }

        /// <summary>
        /// Use the NHibernate saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        public static void SetNHibernateSagaRepositoryProvider(this IRegistrationConfigurator configurator)
        {
            configurator.SetSagaRepositoryProvider(new NHibernateSagaRepositoryRegistrationProvider());
        }
    }
}
