namespace MassTransit
{
    using Saga;


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
            configurator.Repository(x => x.RegisterFactoryMethod(provider => new InMemorySagaRepository<T>()));

            return configurator;
        }
    }
}
