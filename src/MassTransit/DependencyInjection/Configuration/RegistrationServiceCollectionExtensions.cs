namespace MassTransit.Configuration
{
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
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

        internal static void RemoveSagaRepositories(this IServiceCollection collection)
        {
            collection.RemoveAll(typeof(ISagaConsumeContextFactory<,>));
            collection.RemoveAll(typeof(ISagaRepositoryContextFactory<>));

            collection.RemoveAll(typeof(DependencyInjectionSagaRepositoryContextFactory<>));
            collection.RemoveAll(typeof(ISagaRepository<>));
        }
    }
}
