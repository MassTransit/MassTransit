namespace MassTransit
{
    using Registration;
    using Saga;
    using Saga.InMemoryRepository;


    public static class SagaRepositoryRegistrationExtensions
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

        public static void RegisterInMemorySagaRepository<T>(this IContainerRegistrar registrar)
            where T : class, ISaga
        {
            registrar.RegisterSingleInstance(new IndexedSagaDictionary<T>());
            registrar.RegisterSagaRepository<T, IndexedSagaDictionary<T>, InMemorySagaConsumeContextFactory<T>, InMemorySagaRepositoryContextFactory<T>>();
        }
    }
}
