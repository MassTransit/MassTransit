namespace MassTransit
{
    using Lamar;
    using LamarIntegration.Registration;
    using Saga;


    public static class LamarExtensions
    {
        internal static INestedContainer GetNestedContainer(this IContainer container, ConsumeContext context)
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Inject(context);

            return nestedContainer;
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this ServiceRegistry registry)
            where T : class, ISaga
        {
            var registrar = new LamarContainerRegistrar(registry);

            registrar.RegisterInMemorySagaRepository<T>();
        }
    }
}
