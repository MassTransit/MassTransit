namespace MassTransit.Persistence.PostgreSql.Configuration
{
    using System.Data;


    /// <summary>
    /// Builds the appropriate components for a Postgres-based DatabaseContext, specific to JobConsumers.
    /// </summary>
    public interface IPostgresJobSagaRepositoryConfigurator
    {
        /// <summary>
        /// Gets/sets the connection string used by all JobConsumer sagas.
        /// </summary>
        string? ConnectionString { get; set; }

        /// <summary>
        /// Gets/sets the isolation level used by transactions.
        /// </summary>
        IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Sets the connection string used by all JobConsumer sagas.
        /// </summary>
        IPostgresJobSagaRepositoryConfigurator SetConnectionString(string connectionString);

        /// <summary>
        /// Sets the isolation level used by transactions.  Defaults to RepeatableRead.
        /// </summary>
        IPostgresJobSagaRepositoryConfigurator SetIsolationLevel(IsolationLevel isolationLevel);
    }
}
