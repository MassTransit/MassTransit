namespace MassTransit.Persistence.PostgreSql.Connections
{
    using System.Data;
    using System.Runtime.CompilerServices;
    using Integration.Saga;
    using Integration.SqlBuilders;
    using Npgsql;


    public abstract class PostgresDatabaseContext<TSaga> : SagaDatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly string _connectionString;
        protected readonly string IdColumnName;

        protected readonly string TableName;

        bool _disposed;

        protected NpgsqlConnection? Connection;
        protected NpgsqlTransaction? Transaction;

        protected PostgresDatabaseContext(string connectionString, string tableName, string idColumnName)
        {
            _connectionString = connectionString;
            TableName = tableName;
            IdColumnName = idColumnName;

            if (idColumnName != nameof(ISaga.CorrelationId))
                MapProperty(s => s.CorrelationId, idColumnName);
        }

        protected static string BuildQueryPredicate(List<SqlPredicate> predicates, Action<string, object?> parameterCallback)
        {
            var queryPredicates = new List<string>();

            foreach (var p in predicates)
            {
                var paramName = p.Name.ToLowerInvariant();

                queryPredicates.Add($"{p.Name} {p.Operator} @{paramName}");
                parameterCallback?.Invoke($"@{paramName}", p.Value);
            }

            return string.Join(" AND ", queryPredicates);
        }

        protected override async IAsyncEnumerable<TSaga> ReadAsync(string sql, object? parameters, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var readerAdapter = CreateReaderAdapter();
            var writerAdapter = CreateWriterAdapter();

            NpgsqlCommand? command = null;

            try
            {
                command = await CreateCommand(sql, cancellationToken)
                    .ConfigureAwait(false);

                writerAdapter(parameters, command.Parameters);

                await OnParametersWritten(command, cancellationToken)
                    .ConfigureAwait(false);

                await using var reader = await command.ExecuteReaderAsync(cancellationToken)
                    .ConfigureAwait(false);

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    yield return readerAdapter(reader);
            }
            finally
            {
                if (command is not null)
                    await command.DisposeAsync().ConfigureAwait(false);
            }
        }

        protected override async Task<int> ExecuteAsync(string sql, object? parameters, CancellationToken cancellationToken)
        {
            var writerAdapter = CreateWriterAdapter();

            NpgsqlCommand? command = null;
            try
            {
                command = await CreateCommand(sql, cancellationToken)
                    .ConfigureAwait(false);

                writerAdapter(parameters, command.Parameters);

                await OnParametersWritten(command, cancellationToken)
                    .ConfigureAwait(false);

                var rows = await command.ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);

                return rows;
            }
            finally
            {
                if (command is not null)
                    await command.DisposeAsync().ConfigureAwait(false);
            }
        }

        protected virtual async Task<NpgsqlCommand> CreateCommand(string sql, CancellationToken cancellationToken)
        {
            Connection ??= await CreateConnection(cancellationToken)
                .ConfigureAwait(false);

            var command = Connection.CreateCommand();

            if (Transaction is not null)
                command.Transaction = Transaction;

            command.CommandText = sql;

            return command;
        }

        protected virtual async Task<NpgsqlConnection> CreateConnection(CancellationToken cancellationToken)
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken)
                .ConfigureAwait(false);

            await OnConnectionOpened(connection, cancellationToken)
                .ConfigureAwait(false);

            return connection;
        }

        /// <summary>
        /// Reader adapters are to convert from an individual IDataReader row from a
        /// database reader to a hydrated model instance.  Defaults to a generic runtime
        /// adapter, but can be overridden for performance or complex mappings.
        /// </summary>
        protected virtual Func<IDataReader, TSaga> CreateReaderAdapter()
        {
            return ReflectionsAdapter.CreateFor<TSaga>();
        }

        /// <summary>
        /// Writer adapters are to convert an object (usually model instance) to a
        /// parameter collection for sending to the database.  The default writer will
        /// have engine-specific logic, but a custom writer can be used for any saga
        /// that needs non-trivial logic.
        /// </summary>
        protected virtual Action<object?, NpgsqlParameterCollection> CreateWriterAdapter()
        {
            return AssignParameters;
        }

        /// <summary>
        /// Called immediately after a new connection is opened.  Generally used to begin a transaction
        /// or set additional properties on the connection, such as buffer sizes or attaching event handlers.
        /// </summary>
        protected virtual ValueTask OnConnectionOpened(NpgsqlConnection connection, CancellationToken cancellationToken)
        {
            return CompletedTask;
        }

        /// <summary>
        /// Called immediately after parameters are written.  Generally used by specific types of database
        /// engines to handle special property types.
        /// </summary>
        protected virtual ValueTask OnParametersWritten(NpgsqlCommand command, CancellationToken cancellationToken)
        {
            return CompletedTask;
        }

        protected static void AssignParameters(object? parameters, NpgsqlParameterCollection collection)
        {
            foreach (var (name, value) in ParameterReader.Read(parameters))
                collection.AddWithValue(name, value ?? DBNull.Value);
        }

        public virtual Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Transaction is null
                ? Task.CompletedTask
                : Transaction.CommitAsync(cancellationToken);
        }

        public virtual void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            Transaction?.Dispose();
            Connection?.Dispose();
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (Transaction is not null)
                await Transaction.DisposeAsync().ConfigureAwait(false);

            if (Connection is not null)
                await Connection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
