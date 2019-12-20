namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Configuration
{
    using System;
    using System.Linq;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Registration;


    public interface IEntityFrameworkSagaRepositoryConfigurator
    {
        ConcurrencyMode ConcurrencyMode { set; }

        /// <summary>
        /// Use already registered DbContext
        /// </summary>
        void AddExistingDbContext<TContext>()
            where TContext : DbContext;

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
