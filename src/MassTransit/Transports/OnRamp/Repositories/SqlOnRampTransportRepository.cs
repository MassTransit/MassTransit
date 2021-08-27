using MassTransit.Transports.OnRamp.Configuration;
using Newtonsoft.Json;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp.Repositories
{
    public class SqlOnRampTransportRepository : IOnRampTransportRepository
    {
        private readonly IOnRampDbTransactionContext _transactionContextAccessor;
        private readonly IOnRampTransportOptions _onRampTransportOptions;
        private readonly IOnRampTransportRepositoryStatementProvider _statementProvider;

        public SqlOnRampTransportRepository(
            IOnRampDbTransactionContext transactionContextAccessor,
            IOnRampTransportOptions onRampTransportOptions,
            IOnRampTransportRepositoryStatementProvider statementProvider)
        {
            _transactionContextAccessor = transactionContextAccessor;
            _onRampTransportOptions = onRampTransportOptions;
            _statementProvider = statementProvider;
        }

        public async Task InsertMessage(OnRampSerializedMessage message, CancellationToken cancellationToken = default)
        {
            var transaction = _transactionContextAccessor.Transaction as DbTransaction ?? throw new ArgumentException("Transaction was not set in the IDbTransactionContextAccessor", nameof(_transactionContextAccessor.Transaction));

            using var cmd = transaction.Connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OnRampName", _onRampTransportOptions.OnRampName),
                cmd.CreateParameter("@Id", NewId.NextGuid()),
                cmd.CreateParameter("@InstanceId", DBNull.Value),
                cmd.CreateParameter("@Retries", 0),
                cmd.CreateParameter("@SerializedMessage", JsonConvert.SerializeObject(message, Formatting.None)),
                cmd.CreateParameter("@Added", DateTime.UtcNow.Ticks),
            };

            var sql = _statementProvider.InsertMessageStatement();

            await transaction.Connection.ExecuteNonQueryAsync(sql, transaction, sqlParams, cancellationToken);
        }
    }
}
