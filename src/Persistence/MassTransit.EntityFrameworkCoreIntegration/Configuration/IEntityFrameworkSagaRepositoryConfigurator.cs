namespace MassTransit
{
    using System;
    using System.Data;
    using System.Linq;
    using EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;


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
        void AddDbContext<TContext, TImplementation>(Action<IServiceProvider, DbContextOptionsBuilder<TImplementation>> optionsAction = null)
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
        void DatabaseFactory(Func<IServiceProvider, Func<DbContext>> databaseFactory);

        /// <summary>
        /// Use an existing (already configured in the container) DbContext that will be resolved
        /// within the container scope
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        void ExistingDbContext<TContext>()
            where TContext : DbContext;
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
