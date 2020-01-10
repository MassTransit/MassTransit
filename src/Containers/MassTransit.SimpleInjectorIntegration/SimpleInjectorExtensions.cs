namespace MassTransit
{
    using Saga;
    using SimpleInjector;
    using SimpleInjectorIntegration.Registration;


    public static class SimpleInjectorExtensions
    {
        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this Container registry)
            where T : class, ISaga
        {
            var registrar = new SimpleInjectorContainerRegistrar(registry);

            registrar.RegisterInMemorySagaRepository<T>();
        }
    }
}
