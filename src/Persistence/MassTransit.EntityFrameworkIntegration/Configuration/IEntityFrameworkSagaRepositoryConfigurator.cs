namespace MassTransit
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using EntityFrameworkIntegration;


    public interface IEntityFrameworkSagaRepositoryConfigurator
    {
        ConcurrencyMode ConcurrencyMode { set; }
        IsolationLevel IsolationLevel { set; }
        ILockStatementProvider LockStatementProvider { set; }

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
