namespace MassTransit
{
#if !NET8_0_OR_GREATER

    using System.Data;
    using Npgsql;
    
    public static class NpgsqlExtensions
    {
        public static Task<NpgsqlTransaction> BeginTransactionAsync(
            this NpgsqlConnection connection,
            IsolationLevel isolationLevel = IsolationLevel.RepeatableRead,
            CancellationToken cancellationToken = default)
        {
            return Task.Run(() => connection.BeginTransaction(isolationLevel), cancellationToken);
        }

        public static ValueTask DisposeAsync(this NpgsqlCommand? command)
        {
            command?.Dispose();
            return default;
        }

        public static ValueTask DisposeAsync(this NpgsqlTransaction? transaction)
        {
            transaction?.Dispose();
            return default;
        }
    }
#endif
}
