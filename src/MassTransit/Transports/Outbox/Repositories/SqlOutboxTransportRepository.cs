using MassTransit.Serialization;
using MassTransit.Transports.Outbox.Configuration;
using Newtonsoft.Json;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox.Repositories
{
    public class SqlOutboxTransportRepository : IOutboxTransportRepository
    {
        private readonly IOutboxDbTransactionContext _transactionContextAccessor;
        private readonly IOutboxTransportOptions _outboxTransportOptions;
        private readonly IOutboxTransportRepositoryStatementProvider _statementProvider;

        public SqlOutboxTransportRepository(
            IOutboxDbTransactionContext transactionContextAccessor,
            IOutboxTransportOptions outboxTransportOptions,
            IOutboxTransportRepositoryStatementProvider statementProvider)
        {
            _transactionContextAccessor = transactionContextAccessor;
            _outboxTransportOptions = outboxTransportOptions;
            _statementProvider = statementProvider;
        }

        public async Task InsertMessage(JsonSerializedMessage message, CancellationToken cancellationToken = default)
        {
            var transaction = _transactionContextAccessor.Transaction as DbTransaction ?? throw new ArgumentException("Transaction was not set in the IDbTransactionContextAccessor", nameof(_transactionContextAccessor.Transaction));

            using var cmd = transaction.Connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", _outboxTransportOptions.OutboxName),
                cmd.CreateParameter("@Id", NewId.NextGuid()),
                cmd.CreateParameter("@InstanceId", DBNull.Value),
                cmd.CreateParameter("@Retries", 0),
                cmd.CreateParameter("@SerializedMessage", JsonConvert.SerializeObject(message, Formatting.None)),
                cmd.CreateParameter("@Added", DateTime.UtcNow.Ticks),
                cmd.CreateParameter("@ExpirationTime", DBNull.Value),
            };

            var sql = _statementProvider.InsertMessageStatement();

            await transaction.Connection.ExecuteNonQueryAsync(sql, transaction, sqlParams, cancellationToken);
        }
    }
}
