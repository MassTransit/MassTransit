namespace MassTransit
{
    using Castle.Windsor;
    using Saga;
    using WindsorIntegration.Registration;


    /// <summary>
    /// Extension methods for the Windsor IoC container.
    /// </summary>
    public static class WindsorContainerExtensions
    {
        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="container"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this IWindsorContainer container)
            where T : class, ISaga
        {
            var registrar = new WindsorContainerRegistrar(container);

            registrar.RegisterInMemorySagaRepository<T>();
        }
    }
}
