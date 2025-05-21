#nullable enable
namespace MassTransit
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Configuration;
    using DapperIntegration.Saga;
    using DapperIntegration.SqlBuilders;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Npgsql;
    using Saga;

    public static partial class DapperSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Wires this saga to use the Dapper repository provider.
        /// </summary>
        /// <param name="configurator">The saga registration configurator</param>
        /// <param name="configure">The saga registration callback</param>
        public static ISagaRegistrationConfigurator<TSaga> DapperRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<IDapperSagaRepositoryConfigurator<TSaga>> configure = null)
            where TSaga : class, ISaga
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return DapperRepository(configurator, string.Empty, configure);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Wires this saga to use the Dapper repository provider, with a shortcut to set the connection string.  This method
        /// only allows for SQL Server and is far less configurable.
        /// </summary>
        /// <param name="configurator">The saga registration configurator</param>
        /// <param name="connectionString"></param>
        /// <param name="configure">The saga registration callback</param>
        [Obsolete("DapperRepository should use the configure-only method", false)]
        public static ISagaRegistrationConfigurator<TSaga> DapperRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            string connectionString, Action<IDapperSagaRepositoryConfigurator<TSaga>> configure = null)
            where TSaga : class, ISaga
        {
            var repositoryConfigurator = new DapperSagaRepositoryConfigurator<TSaga>();

            // connection string will only be set from existing code, since it's now Obsolete
            if (!string.IsNullOrEmpty(connectionString))
                repositoryConfigurator.UseSqlServer(connectionString);

            configure?.Invoke(repositoryConfigurator);

            configurator.Repository(repositoryConfigurator.Register);

            return configurator;
        }
    }

    public interface IDapperSagaRepositoryConfigurator<TSaga> :
        IDapperSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Set the database context factory to allow customization of the Dapper interaction/queries
        /// </summary>
        [Obsolete("Use UseContextFactory() instead", true)]
        DatabaseContextFactory<TSaga> ContextFactory { get; set; }


        /// <summary>
        /// Use a custom SQL formatter to build INSERT/UPDATE/DELETE/SELECT statements for this saga.  If set,
        /// this formatter will be used for the default context factory, implemented by <see cref="SagaDatabaseContext{TSaga}"/>.
        /// It is not necessary if a fully custom context factory is provided via <see cref="UseContextFactory"/>.
        /// </summary>
        /// <param name="factory">The factory for the formatter</param>
        void UseSqlFormatter(Func<IServiceProvider, ISagaSqlFormatter<TSaga>> factory);

        /// <summary>
        /// Use a custom repository for this saga.  Saga repositories are responsible for the actual persistence
        /// of the code models and the storage provider.  A custom repository allows for fully custom behavior at
        /// every step of the saving/loading process.
        /// </summary>
        /// <param name="factory">The factory for the database context</param>
        void UseContextFactory(Func<IServiceProvider, DatabaseContextFactory<TSaga>> factory);
    }


    public class DapperSagaRepositoryConfigurator<TSaga> : DapperSagaRepositoryConfigurator,
        IDapperSagaRepositoryConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        public DapperSagaRepositoryConfigurator()
        {
            ContextFactoryProvider = null;
            SqlBuilderProvider = null;
        }

        [Obsolete("Configure the repository via UseXXX() methods", false)]
        public DapperSagaRepositoryConfigurator(string? connectionString = null, IsolationLevel? isolationLevel = null)
            : this()
        {
            if (connectionString is not null)
                ConnectionString = connectionString;

            if (isolationLevel is not null)
                IsolationLevel = isolationLevel;
        }

        protected Func<IServiceProvider, ISagaSqlFormatter<TSaga>>? SqlBuilderProvider { get; set; }
        protected Func<IServiceProvider, DatabaseContextFactory<TSaga>>? ContextFactoryProvider { get; set; }

        [Obsolete("Configure the ContextFactory via UseContextFactory()", false)]
        public DatabaseContextFactory<TSaga>? ContextFactory { get; set; }

        public void UseSqlFormatter(Func<IServiceProvider, ISagaSqlFormatter<TSaga>> factory) => SqlBuilderProvider = factory;

        public void UseContextFactory(Func<IServiceProvider, DatabaseContextFactory<TSaga>> factory) => ContextFactoryProvider = factory;

        internal void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            // Because there are existing implementations in the wild, the context
            // and sql builders have to be very carefully chosen to avoid breaking
            // anything that might be relying on unexpected behavior of the legacy
            // context.  If any of the "new" properties are set, this is new code
            // and would not need to rely on behavior of the legacy context.
            // 
            // This is accounted for in BuildContextFactory below, and
            // DapperSagaRepositoryContextFactory:CreateDatabaseContext.
            var contextFactory = BuildContextFactory();
            var connectionFactory = BuildConnectionFactory();

            // TODO: Consolidate with DapperJobSagaRepositoryConfigurator!
            configurator.AddOptions<DapperOptions<TSaga>>().Configure(opt =>
            {
                if (Provider != DatabaseProviders.Unspecified && opt.Provider != Provider)
                    opt.Provider = Provider;

                opt.ConnectionString ??= ConnectionString;
                opt.IsolationLevel ??= IsolationLevel;
                opt.IdColumnName ??= IdColumnName;
                opt.TableName ??= TableName;

                opt.SqlBuilderProvider ??= SqlBuilderProvider;
                opt.ContextFactoryProvider ??= contextFactory;
                opt.DbConnectionProvider ??= connectionFactory;
            });

            configurator.RegisterLoadSagaRepository<TSaga, DapperSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterQuerySagaRepository<TSaga, DapperSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>,
                DapperSagaRepositoryContextFactory<TSaga>>();
        }

        Func<IServiceProvider, DatabaseContextFactory<TSaga>>? BuildContextFactory()
        {
            // someone made it easy for us again
            if (ContextFactoryProvider is not null)
                return ContextFactoryProvider;

            // legacy setting override
            #pragma warning disable CS0618 // Type or member is obsolete
            if (ContextFactory is not null)
                return _ => ContextFactory;
            #pragma warning restore CS0618 // Type or member is obsolete

            // null is a special case here, causing resolution with the new
            // SagaDatabaseContext<T> in the RepositoryContextFactory
            return null;
        }

        Func<IServiceProvider, DbConnection> BuildConnectionFactory()
        {
            if (DbConnectionProvider is not null)
                return DbConnectionProvider;

            return Provider == DatabaseProviders.Postgres
                ? Postgres()
                : SqlServer();

            static Func<IServiceProvider, DbConnection> SqlServer() =>
                sp =>
                {
                    var options = sp.GetRequiredService<IOptions<DapperOptions<TSaga>>>().Value;
                    return new SqlConnection(options.ConnectionString);
                };

            static Func<IServiceProvider, DbConnection> Postgres() =>
                sp =>
                {
                    var options = sp.GetRequiredService<IOptions<DapperOptions<TSaga>>>().Value;
                    return new NpgsqlConnection(options.ConnectionString);
                };
        }
    }
}
#nullable restore
