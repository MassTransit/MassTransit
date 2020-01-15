namespace MassTransit.EntityFrameworkCoreIntegration
{
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Saga;


    public static class EntityFrameworkSagaRepositoryRegistrationExtensions
    {
        public static void EntityFrameworkRepository<T>(this ISagaRegistrationConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.Repository(r =>
            {
                r.RegisterScoped<ISagaDbContextFactory<T>, ContainerSagaDbContextFactory<T>>();

                r.RegisterComponents<DbContext, EntityFrameworkSagaConsumeContextFactory<T>, EntityFrameworkSagaRepositoryContextFactory<T>>();
            });
        }
    }
}
