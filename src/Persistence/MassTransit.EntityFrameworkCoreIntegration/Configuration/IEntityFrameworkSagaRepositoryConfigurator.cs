namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Data;
    using System.Linq;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Registration;
    using Saga.Configuration;


    public interface IEntityFrameworkSagaRepositoryConfigurator
    {
        ConcurrencyMode ConcurrencyMode { set; }
        IsolationLevel IsolationLevel { set; }
        ILockStatementProvider LockStatementProvider { set; }

        /// <summary>
        /// Add the DbContext to the container, and configure the repository to use it
        /// </summary>
        /// <param name="optionsAction"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        void AddDbContext<TContext, TImplementation>(Action<IConfigurationServiceProvider, DbContextOptionsBuilder<TImplementation>> optionsAction = null)
            where TContext : DbContext
            where TImplementation : DbContext, TContext;

        /// <summary>
        /// Add the DbContext to the container using a pool, and configure the repository to use it
        /// </summary>
        /// <param name="optionsAction"></param>
        /// <param name="poolSize">The maximum number of pooled objects</param>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        void AddDbContextPool<TContext, TImplementation>(Action<IConfigurationServiceProvider, DbContextOptionsBuilder<TImplementation>> optionsAction,
            int poolSize = 128)
            where TContext : DbContext
            where TImplementation : DbContext, TContext;

        /// <summary>
        /// Use a simple factory method to create the database
        /// </summary>
        /// <param name="databaseFactory"></param>
        void DatabaseFactory(Func<DbContext> databaseFactory);

        /// <summary>
        /// Use the configuration service provider to resolve the database factory
        /// </summary>
        /// <param name="databaseFactory"></param>
        void DatabaseFactory(Func<IConfigurationServiceProvider, Func<DbContext>> databaseFactory);
    }


    public interface IEntityFrameworkSagaRepositoryConfigurator<TSaga> :
        IEntityFrameworkSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Use custom query
        /// </summary>
        /// <param name="queryCustomization"></param>
        void CustomizeQuery(Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization);
    }
}
