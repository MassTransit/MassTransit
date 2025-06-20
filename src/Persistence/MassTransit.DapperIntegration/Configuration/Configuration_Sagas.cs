#nullable enable
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Configuration;
    using DapperIntegration.Saga;

    public static partial class DapperSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Use the Dapper saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        [Obsolete("Configure connection string via the configurator callback", false)]
        public static void SetDapperSagaRepositoryProvider(this IRegistrationConfigurator configurator, string connectionString,
            Action<IDapperSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new DapperSagaRepositoryRegistrationProvider(connectionString, configure));
        }

        /// <summary>
        /// Use the Dapper saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        public static void SetDapperSagaRepositoryProvider(this IRegistrationConfigurator configurator, Action<IDapperSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new DapperSagaRepositoryRegistrationProvider(configure));
        }
    }

    public interface IDapperSagaRepositoryConfigurator
    {
        /// <summary>
        /// Configures the saga to use Microsoft SQL Server with this connection string.
        /// Can also be set with <seealso cref="DapperOptions{TSaga}.ConnectionString"/> and <seealso cref="DapperOptions{TSaga}.Provider"/>.
        /// </summary>
        /// <param name="connectionString">The connection string to use</param>
        void UseSqlServer(string connectionString);

        /// <summary>
        /// Configures the saga to use PostgreSQL with this connection string.
        /// Can also be set with <seealso cref="DapperOptions{TSaga}.ConnectionString"/> and <seealso cref="DapperOptions{TSaga}.Provider"/>.
        /// </summary>
        /// <param name="connectionString">The connection string to use</param>
        void UsePostgres(string connectionString);

        /// <summary>
        /// Configures the saga to use this transaction level.  Defaults to <see cref="System.Data.IsolationLevel.Serializable"/>.
        /// Can also be set with <seealso cref="DapperOptions{TSaga}.IsolationLevel"/>.
        /// </summary>
        /// <param name="isolationLevel">The isolation level to use for all operations for this saga</param>
        void UseIsolationLevel(IsolationLevel isolationLevel);

        /// <summary>
        /// Explicitly specify the table to use.  Takes precedence over `[Table("table_name")]` or pluralizing the saga name.
        /// </summary>
        /// <param name="tableName">The exact table name to use</param>
        void UseTableName(string tableName);

        /// <summary>
        /// Explicitly specify the name of the primary key column.  Takes precedence over `[Key]`/`[ExplicitKey]` or "CorrelationId".
        /// </summary>
        void UseIdColumnName(string idColumnName);

        /// <summary>
        /// Specify an explicit factory method to create database connections. The connection does
        /// not need to be opened prior to returning it.
        /// </summary>
        /// <param name="factory">The connection factory to use</param>
        void UseDbConnectionProvider(Func<IServiceProvider, DbConnection> factory);
    }
    
    public class DapperSagaRepositoryConfigurator : IDapperSagaRepositoryConfigurator,
        ISpecification
    {
        protected DatabaseProviders Provider = DatabaseProviders.Unspecified;

        protected string? ConnectionString { get; set; }
        protected string? TableName { get; set; }
        protected string? IdColumnName { get; set; }
        protected IsolationLevel? IsolationLevel { get; set; }
        protected Func<IServiceProvider, DbConnection>? DbConnectionProvider { get; set; }

        public void UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
            Provider = DatabaseProviders.SqlServer;
        }

        public void UsePostgres(string connectionString)
        {
            ConnectionString = connectionString;
            Provider = DatabaseProviders.Postgres;
        }

        public void UseIsolationLevel(IsolationLevel isolationLevel) => IsolationLevel = isolationLevel;

        public void UseTableName(string tableName) => TableName = tableName;

        public void UseIdColumnName(string idColumnName) => IdColumnName = idColumnName;

        public void UseDbConnectionProvider(Func<IServiceProvider, DbConnection> factory) => DbConnectionProvider = factory;

        public IEnumerable<ValidationResult> Validate()
        {
            // validation has to be choosy because it is possible to configure
            // everything entirely with IOptions<DapperOptions<TSaga>>, so there would
            // be nothing to validate here.
            //
            // any remaining validation can happen as an IValidateOptions<DapperOptions<TSaga>>

            if (AnythingChanged())
            {
                if (string.IsNullOrWhiteSpace(ConnectionString) && DbConnectionProvider is null)
                    yield return this.Failure("ConnectionString", "must be specified");
            }

            yield break;

            bool AnythingChanged() =>
                Provider != DatabaseProviders.Unspecified
                || IsolationLevel is not null
                || !string.IsNullOrEmpty(TableName)
                || !string.IsNullOrEmpty(IdColumnName);
        }
    }

    public class DapperSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<IDapperSagaRepositoryConfigurator> _configure;
        readonly string _connectionString;

        public DapperSagaRepositoryRegistrationProvider(Action<IDapperSagaRepositoryConfigurator> configure)
        {
            _connectionString = string.Empty;
            _configure = configure;
        }

        [Obsolete("Set connection string via the configurator callback or IOptions<DapperOptions<TSaga>>", false)]
        public DapperSagaRepositoryRegistrationProvider(string connectionString, Action<IDapperSagaRepositoryConfigurator> configure)
        {
            _connectionString = connectionString;
            _configure = configure;
        }

        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            configurator.DapperRepository(r =>
            {
                if (!string.IsNullOrEmpty(_connectionString))
                    r.UseSqlServer(_connectionString);

                _configure?.Invoke(r);
            });
        }
    }
}
#nullable restore
