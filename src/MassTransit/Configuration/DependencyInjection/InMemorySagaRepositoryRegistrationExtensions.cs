namespace MassTransit
{
    using Configuration;


    public static class InMemorySagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds an in-memory saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> InMemoryRepository<T>(this ISagaRegistrationConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.Repository(x => x.RegisterInMemorySagaRepository<T>());

            return configurator;
        }

        /// <summary>
        /// Use the InMemorySagaRepository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        public static void SetInMemorySagaRepositoryProvider(this IRegistrationConfigurator configurator)
        {
            configurator.SetSagaRepositoryProvider(new InMemorySagaRepositoryRegistrationProvider());
        }
    }
}
