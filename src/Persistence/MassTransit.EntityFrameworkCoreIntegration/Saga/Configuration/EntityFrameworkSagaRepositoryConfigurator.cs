namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Registration;


    class EntityFrameworkSagaRepositoryConfigurator<TSaga> :
        IEntityFrameworkSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        ConcurrencyMode _concurrencyMode;
        Func<IConfigurationServiceProvider, Func<DbContext>> _databaseFactory;
        Func<IQueryable<TSaga>, IQueryable<TSaga>> _queryCustomization;

        public void CustomizeQuery(Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization)
        {
            _queryCustomization = queryCustomization;
        }

        public ConcurrencyMode ConcurrencyMode
        {
            set => _concurrencyMode = value;
        }

        public void AddExistingDbContext<TContext>()
            where TContext : DbContext
        {
            DatabaseFactory(provider => provider.GetService<TContext>);
        }

        public void DatabaseFactory(Func<DbContext> databaseFactory)
        {
            DatabaseFactory(_ => databaseFactory);
        }

        public void DatabaseFactory(Func<IConfigurationServiceProvider, Func<DbContext>> databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_databaseFactory == null)
                yield return this.Failure("DatabaseFactory", "must be specified");
        }

        public Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> BuildFactoryMethod()
        {
            ISagaRepository<TSaga> CreateRepository(IConfigurationServiceProvider provider)
            {
                Func<DbContext> databaseFactory = _databaseFactory(provider);

                return _concurrencyMode switch
                {
                    ConcurrencyMode.Optimistic => EntityFrameworkSagaRepository<TSaga>.CreateOptimistic(databaseFactory, _queryCustomization),
                    ConcurrencyMode.Pessimistic =>
                    EntityFrameworkSagaRepository<TSaga>.CreatePessimistic(databaseFactory, queryCustomization: _queryCustomization),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return CreateRepository;
        }
    }
}
