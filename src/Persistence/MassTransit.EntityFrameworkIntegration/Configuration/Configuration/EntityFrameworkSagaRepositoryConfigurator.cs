namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using EntityFrameworkIntegration;
    using EntityFrameworkIntegration.Saga;
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

        public void DatabaseFactory(Func<DbContext> databaseFactory)
        {
            DatabaseFactory(_ => databaseFactory);
        }

        public void DatabaseFactory(Func<IServiceProvider, Func<DbContext>> databaseFactory)
        {
            _configureDbContext = configurator =>
            {
                configurator.TryAddScoped(provider => databaseFactory(provider)());

                configurator.TryAddScoped<ISagaDbContextFactory<TSaga>, ContainerSagaDbContextFactory<DbContext, TSaga>>();
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
