namespace MassTransit.Persistence.SqlServer.Configuration
{
    using System.Data;


    /// <summary>
    /// Builds the appropriate components for a SQL Server-based DatabaseContext, specific to JobConsumers.
    /// </summary>
    public interface ISqlServerJobSagaRepositoryConfigurator
    {
        /// <summary>
        /// Gets/sets the connection string used by all JobConsumer sagas.
        /// </summary>
        string? ConnectionString { get; set; }

        /// <summary>
        /// Gets/sets the isolation level for transactions.
        /// </summary>
        IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Sets the connection string used by all JobConsumer sagas.
        /// </summary>
        ISqlServerJobSagaRepositoryConfigurator SetConnectionString(string connectionString);

        /// <summary>
        /// Sets the isolation level for transactions (defaults to RepeatableRead).
        /// </summary>
        ISqlServerJobSagaRepositoryConfigurator SetIsolationLevel(IsolationLevel isolationLevel);
    }
}
