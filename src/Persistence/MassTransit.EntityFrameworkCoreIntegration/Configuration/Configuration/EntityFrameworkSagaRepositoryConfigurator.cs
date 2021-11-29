namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using EntityFrameworkCoreIntegration;
    using EntityFrameworkCoreIntegration.Saga;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Saga;


    public class EntityFrameworkSagaRepositoryConfigurator<TSaga> :
        IEntityFrameworkSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        ConcurrencyMode _concurrencyMode;
        Action<ISagaRepositoryRegistrationConfigurator<TSaga>> _configureDbContext;
        IsolationLevel _isolationLevel;
        ILockStatementProvider _lockStatementProvider;
        Func<IQueryable<TSaga>, IQueryable<TSaga>> _queryCustomization;

        public EntityFrameworkSagaRepositoryConfigurator()
        {
            _isolationLevel = IsolationLevel.Serializable;
            _concurrencyMode = ConcurrencyMode.Pessimistic;
            _lockStatementProvider = new SqlServerLockStatementProvider();
        }

        public IsolationLevel IsolationLevel
        {
            set => _isolationLevel = value;
        }

        public void CustomizeQuery(Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization)
        {
            _queryCustomization = queryCustomization;
        }

        public ConcurrencyMode ConcurrencyMode
        {
            set
            {
                _concurrencyMode = value;
                if (_concurrencyMode == ConcurrencyMode.Optimistic && _isolationLevel == IsolationLevel.Serializable)
                    _isolationLevel = IsolationLevel.ReadCommitted;
            }
        }

        public ILockStatementProvider LockStatementProvider
        {
            set => _lockStatementProvider = value;
        }

        public void AddDbContext<TContext, TImplementation>(Action<IServiceProvider, DbContextOptionsBuilder<TImplementation>> optionsAction)
            where TContext : DbContext
            where TImplementation : DbContext, TContext
        {
            _configureDbContext = configurator =>
            {
                AddDbContext<TContext, TImplementation>(configurator, optionsAction);
            };
        }

        public void DatabaseFactory(Func<DbContext> databaseFactory)
        {
            DatabaseFactory(_ => databaseFactory);
        }

        public void DatabaseFactory(Func<IServiceProvider, Func<DbContext>> databaseFactory)
        {
            _configureDbContext = configurator =>
            {
                configurator.TryAddScoped<ISagaDbContextFactory<TSaga>>(provider => new DelegateSagaDbContextFactory<TSaga>(databaseFactory(provider)));
            };
        }

        public void ExistingDbContext<TContext>()
            where TContext : DbContext
        {
            _configureDbContext = configurator =>
            {
                configurator.TryAddScoped<ISagaDbContextFactory<TSaga>, ContainerSagaDbContextFactory<TContext, TSaga>>();
            };
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_configureDbContext == null)
                yield return this.Failure("DbContext", "must be specified");
        }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            _configureDbContext?.Invoke(configurator);

            if (_concurrencyMode == ConcurrencyMode.Optimistic)
                configurator.TryAddSingleton(provider => CreateOptimisticLockStrategy());
            else
                configurator.TryAddSingleton(provider => CreatePessimisticLockStrategy());

            configurator.RegisterSagaRepository<TSaga, DbContext, SagaConsumeContextFactory<DbContext, TSaga>,
                EntityFrameworkSagaRepositoryContextFactory<TSaga>>();
        }

        static void AddDbContext<TContext, TImplementation>(IServiceCollection collection,
            Action<IServiceProvider, DbContextOptionsBuilder<TImplementation>> optionsAction)
            where TImplementation : DbContext, TContext
            where TContext : DbContext
        {
            if (optionsAction != null)
                CheckContextConstructors<TImplementation>();

            collection.TryAddSingleton(provider => DbContextOptionsFactory(provider, optionsAction));
            collection.TryAddScoped<DbContextOptions>(provider => provider.GetRequiredService<DbContextOptions<TImplementation>>());

            collection.TryAddScoped<TContext, TImplementation>();

            collection.TryAddScoped<ISagaDbContextFactory<TSaga>, ContainerSagaDbContextFactory<TContext, TSaga>>();
        }

        static DbContextOptions<TContext> DbContextOptionsFactory<TContext>(IServiceProvider provider,
            Action<IServiceProvider, DbContextOptionsBuilder<TContext>> optionsAction)
            where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>(new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

            builder.UseApplicationServiceProvider(provider);

            optionsAction?.Invoke(provider, builder);

            return builder.Options;
        }

        static void CheckContextConstructors<TContext>()
            where TContext : DbContext
        {
            List<ConstructorInfo> declaredConstructors = typeof(TContext).GetTypeInfo().DeclaredConstructors.ToList();
            if (declaredConstructors.Count == 1 && declaredConstructors[0].GetParameters().Length == 0)
                throw new ArgumentException(CoreStrings.DbContextMissingConstructor(typeof(TContext).ShortDisplayName()));
        }

        ISagaRepositoryLockStrategy<TSaga> CreateOptimisticLockStrategy()
        {
            ILoadQueryProvider<TSaga> queryProvider = new DefaultSagaLoadQueryProvider<TSaga>();
            if (_queryCustomization != null)
                queryProvider = new CustomSagaLoadQueryProvider<TSaga>(queryProvider, _queryCustomization);

            var queryExecutor = new OptimisticLoadQueryExecutor<TSaga>(queryProvider);

            return new OptimisticSagaRepositoryLockStrategy<TSaga>(queryProvider, queryExecutor, _isolationLevel);
        }

        ISagaRepositoryLockStrategy<TSaga> CreatePessimisticLockStrategy()
        {
            var statementProvider = _lockStatementProvider ?? new SqlServerLockStatementProvider();

            var queryExecutor = new PessimisticLoadQueryExecutor<TSaga>(statementProvider, _queryCustomization);

            return new PessimisticSagaRepositoryLockStrategy<TSaga>(queryExecutor, _isolationLevel);
        }
    }
}
