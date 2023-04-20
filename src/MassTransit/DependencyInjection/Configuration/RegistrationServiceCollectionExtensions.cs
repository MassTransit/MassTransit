namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
            collection.TryAddSingleton<ISagaRepository<TSaga>, TempSagaRepository<TSaga>>();
            collection.TryAddSingleton<IQuerySagaRepository<TSaga>, NotSupportedSagaRepository<TSaga>>();
            collection.TryAddSingleton<ILoadSagaRepository<TSaga>, NotSupportedSagaRepository<TSaga>>();
        }

        public static void RegisterQuerySagaRepository<TSaga, TQueryRepositoryContextFactory>(this IServiceCollection collection)
            where TSaga : class, ISaga
            where TQueryRepositoryContextFactory : class, IQuerySagaRepositoryContextFactory<TSaga>
        {
            collection.AddSingleton<IQuerySagaRepository<TSaga>, DependencyInjectionQuerySagaRepository<TSaga>>();
            collection.AddScoped<IQuerySagaRepositoryContextFactory<TSaga>, TQueryRepositoryContextFactory>();
        }

        public static void RegisterLoadSagaRepository<TSaga, TLoadRepositoryContextFactory>(this IServiceCollection collection)
            where TSaga : class, ISaga
            where TLoadRepositoryContextFactory : class, ILoadSagaRepositoryContextFactory<TSaga>
        {
            collection.AddSingleton<ILoadSagaRepository<TSaga>, DependencyInjectionLoadSagaRepository<TSaga>>();
            collection.AddScoped<ILoadSagaRepositoryContextFactory<TSaga>, TLoadRepositoryContextFactory>();
        }

        internal static void RemoveSagaRepositories(this IServiceCollection collection)
        {
            collection.RemoveAll(typeof(ISagaConsumeContextFactory<,>));
            collection.RemoveAll(typeof(ISagaRepositoryContextFactory<>));
            collection.RemoveAll(typeof(IQuerySagaRepositoryContextFactory<>));
            collection.RemoveAll(typeof(ILoadSagaRepositoryContextFactory<>));

            collection.RemoveAll(typeof(IQuerySagaRepository<>));
            collection.RemoveAll(typeof(ILoadSagaRepository<>));
            collection.RemoveAll(typeof(ISagaRepository<>));
        }


        class TempSagaRepository<TSaga> :
            ISagaRepository<TSaga>,
            IQuerySagaRepository<TSaga>,
            ILoadSagaRepository<TSaga>
            where TSaga : class, ISaga
        {
            const string SendSagaIsNotAvailableInIocAnymore = "Send saga is not available in IoC anymore";
            readonly ILoadSagaRepository<TSaga> _loadSagaRepository;
            readonly IQuerySagaRepository<TSaga> _querySagaRepository;

            public TempSagaRepository(IQuerySagaRepository<TSaga> querySagaRepository, ILoadSagaRepository<TSaga> loadSagaRepository)
            {
                _querySagaRepository = querySagaRepository;
                _loadSagaRepository = loadSagaRepository;
            }

            public Task<TSaga> Load(Guid correlationId)
            {
                return _loadSagaRepository.Load(correlationId);
            }

            public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
            {
                return _querySagaRepository.Find(query);
            }

            public void Probe(ProbeContext context)
            {
                var scope = context.CreateScope("sagaRepository");

                _querySagaRepository.Probe(scope);
                _loadSagaRepository.Probe(scope);
            }

            public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
                where T : class
            {
                throw new NotSupportedException(SendSagaIsNotAvailableInIocAnymore);
            }

            public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
                where T : class
            {
                throw new NotSupportedException(SendSagaIsNotAvailableInIocAnymore);
            }
        }


        class NotSupportedSagaRepository<TSaga> :
            IQuerySagaRepository<TSaga>,
            ILoadSagaRepository<TSaga>
            where TSaga : class, ISaga
        {
            static readonly string QueryErrorMessage =
                $"Query-based saga correlation is not available when using current saga repository implementation: {TypeCache<TSaga>.ShortName}";

            static readonly string LoadErrorMessage =
                $"Load-based saga correlation is not available when using current saga repository implementation: {TypeCache<TSaga>.ShortName}";

            public Task<TSaga> Load(Guid correlationId)
            {
                throw new NotSupportedException(LoadErrorMessage);
            }

            public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
            {
                throw new NotSupportedException(QueryErrorMessage);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
