namespace MassTransit.Persistence.PostgreSql.Configuration
{
    using System.Data;


    public class PostgresMessageDataConfigurator : IPostgresMessageDataConfigurator,
        ISpecification
    {
        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                yield return this.Failure($"{nameof(ConnectionString)} must be set");

            if (string.IsNullOrWhiteSpace(TableName))
                yield return this.Failure($"{nameof(TableName)} must be set");

            if (string.IsNullOrWhiteSpace(IdColumnName))
                yield return this.Failure($"{nameof(IdColumnName)} must be set");
        }

        /// <inheritdoc />
        public string? ConnectionString { get; set; }

        /// <inheritdoc />
        public string TableName { get; set; } = "MessageData";

        /// <inheritdoc />
        public string IdColumnName { get; set; } = "Id";

        /// <inheritdoc />
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.RepeatableRead;

        /// <inheritdoc />
        public IPostgresMessageDataConfigurator SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
            return this;
        }

        /// <inheritdoc />
        public IPostgresMessageDataConfigurator SetTableName(string tableName)
        {
            TableName = tableName;
            return this;
        }

        /// <inheritdoc />
        public IPostgresMessageDataConfigurator SetIdColumnName(string idColumnName)
        {
            IdColumnName = idColumnName;
            return this;
        }

        /// <inheritdoc />
        public IPostgresMessageDataConfigurator SetIsolationLevel(IsolationLevel isolationLevel)
        {
            IsolationLevel = isolationLevel;
            return this;
        }
    }
}
