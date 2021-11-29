namespace MassTransit.Configuration
{
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;


    public static class RegistrationServiceCollectionExtensions
    {
        public static void RegisterSagaRepository<TSaga, TContext, TConsumeContextFactory, TRepositoryContextFactory>(this IServiceCollection collection)
            where TSaga : class, ISaga
            where TContext : class
            where TConsumeContextFactory : class, ISagaConsumeContextFactory<TContext, TSaga>
            where TRepositoryContextFactory : class, ISagaRepositoryContextFactory<TSaga>
        {
            collection.AddScoped<ISagaConsumeContextFactory<TContext, TSaga>, TConsumeContextFactory>();
            collection.AddScoped<ISagaRepositoryContextFactory<TSaga>, TRepositoryContextFactory>();

            collection.AddSingleton<DependencyInjectionSagaRepositoryContextFactory<TSaga>>();
            collection.AddSingleton<ISagaRepository<TSaga>>(provider =>
                new SagaRepository<TSaga>(provider.GetRequiredService<DependencyInjectionSagaRepositoryContextFactory<TSaga>>()));
        }
    }
}
