namespace MassTransit.Persistence.PostgreSql.Configuration
{
    using System.Data;
    using System.Linq.Expressions;


    /// <summary>
    /// Configures Postgres-specific settings for an individual saga repository.
    /// </summary>
    public interface IPostgresRepositoryConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Gets/sets the connection string.
        /// </summary>
        string? ConnectionString { get; set; }

        /// <summary>
        /// Gets/sets the concurrency mode.
        /// </summary>
        ConcurrencyMode ConcurrencyMode { get; set; }

        /// <summary>
        /// Gets/sets the transaction isolation level for pessimistic concurrency.
        /// </summary>
        IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Gets/sets the model version property for optimistic concurrency.
        /// </summary>
        string? VersionPropertyName { get; set; }

        /// <summary>
        /// Gets/sets the name of the table.  Defaults to the plural form of the saga name if
        /// unspecified, such as `class UserSaga -> table UserSagas`.
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// Gets/sets the primary key column name.  Defaults to `CorrelationId` if unspecified.  Respects [Key] and [ExplicitKey] attributes.
        /// </summary>
        string IdentityColumnName { get; set; }

        /// <summary>
        /// Sets the connection string for this saga.
        /// </summary>
        IPostgresRepositoryConfigurator<TSaga> SetConnectionString(string connectionString);

        /// <summary>
        /// Sets the table name for this saga.  No schemas are assumed, and the table name will
        /// be auto-detected from attributes or naive pluralization if unspecified.
        /// </summary>
        IPostgresRepositoryConfigurator<TSaga> SetTableName(string tableName);

        /// <summary>
        /// Sets the name of the primary key column.  Defaults to `CorrelationId` if not specified.  Respects [Key] and [ExplicitKey] attributes.
        /// </summary>
        IPostgresRepositoryConfigurator<TSaga> SetIdentityColumnName(string identityColumnName);

        /// <summary>
        /// Use optimistic concurrency for saga operations.  Requires a versioning column in the model.
        /// </summary>
        IPostgresRepositoryConfigurator<TSaga> SetOptimisticConcurrency<TProp>(Expression<Func<TSaga, TProp>> versionPropertySelector);

        /// <summary>
        /// Use optimistic concurrency for saga operations.  Requires a versioning column in the model.
        /// </summary>
        IPostgresRepositoryConfigurator<TSaga> SetOptimisticConcurrency(string versionPropertyName = "XMin");

        /// <summary>
        /// Use pessimistic concurrency for saga operations, the default.  Does not require versioning,
        /// but will lock rows being used.  <see cref="IsolationLevel" /> sets the isolation for the
        /// transaction.
        /// </summary>
        IPostgresRepositoryConfigurator<TSaga> SetPessimisticConcurrency(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
