namespace MassTransit.Persistence.MySql.Configuration
{
    using System.Data;


    /// <summary>
    /// Builds the appropriate components for a MySql-based DatabaseContext, specific to JobConsumers.
    /// </summary>
    public interface IMySqlJobSagaRepositoryConfigurator
    {
        /// <summary>
        /// Gets/sets the connection string used by all JobConsumer sagas.
        /// </summary>
        string? ConnectionString { get; set; }

        /// <summary>
        /// Gets/sets the isolation level used by transactions.  Defaults to RepeatableRead.
        /// </summary>
        IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Sets the connection string used by all JobConsumer sagas.
        /// </summary>
        IMySqlJobSagaRepositoryConfigurator SetConnectionString(string connectionString);

        /// <summary>
        /// Sets the isolation level used by transactions.
        /// </summary>
        IMySqlJobSagaRepositoryConfigurator SetIsolationLevel(IsolationLevel isolationLevel);
    }
}
