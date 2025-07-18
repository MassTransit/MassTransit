namespace MassTransit
{
#if !NET8_0_OR_GREATER

    using System.Data;
    using Microsoft.Data.SqlClient;

    public static class SqlExtensions
    {
        public static Task<SqlTransaction> BeginTransactionAsync(
            this SqlConnection connection,
            IsolationLevel isolationLevel = IsolationLevel.RepeatableRead,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(connection.BeginTransaction(isolationLevel));
        }

        public static ValueTask DisposeAsync(this SqlCommand? command)
        {
            command?.Dispose();
            return default;
        }

        public static ValueTask DisposeAsync(this SqlConnection? connection)
        {
            connection?.Dispose();
            return default;
        }

        public static ValueTask DisposeAsync(this SqlTransaction? transaction)
        {
            transaction?.Dispose();
            return default;
        }

        public static ValueTask DisposeAsync(this SqlDataReader? reader)
        {
            reader?.Dispose();
            return default;
        }
        
        public static Task CommitAsync(this SqlTransaction? transaction, CancellationToken cancellationToken = default)
        {
            transaction?.Commit();
            return Task.CompletedTask;
        }

        public static ValueTask RollbackAsync(this SqlTransaction? transaction, CancellationToken cancellationToken = default)
        {
            transaction?.Rollback();
            return default;
        }
    }
#endif
}
