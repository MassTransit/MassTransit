using MassTransit.Serialization;
using MassTransit.Transports.Outbox.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox.Repositories
{
    public class SqlSweeperRepository : SqlLockRepository, ISweeperRepository
    {
        private readonly ISweeperRepositoryStatementProvider _statementProvider;

        public SqlSweeperRepository(
            DbConnection dbConnection,
            ISweeperRepositoryStatementProvider statementProvider,
            ILockRepositoryStatementProvider lockRepositoryStatementProvider)
            : base(dbConnection, lockRepositoryStatementProvider)
        {
            _statementProvider = statementProvider;
        }

        public async Task FailedToSendMessage(OutboxSweeperSendException exception, CancellationToken cancellationToken = default)
        {
            // Do I want to log these exceptions?
            var message = exception.OutboxMessage;

            var sql = _statementProvider.FailedToSendMessageStatement();

            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", message.OutboxName),
                cmd.CreateParameter("@Id", message.Id),
            };

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);
        }

        public async Task FailedToSendMessages(List<OutboxMessage> failedMessages, string instanceId, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", failedMessages[0].OutboxName),
                cmd.CreateParameter("@Instance", instanceId),
            };

            var sql = _statementProvider.FailedToSendMessagesStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);
        }

        public async Task<IReadOnlyList<OutboxMessage>> FetchNextMessages(string outboxName, int prefetchCount, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                    cmd.CreateParameter("@OutboxName", outboxName),
            };

            var sql = _statementProvider.FetchNextMessagesStatement();

            List<OutboxMessage> result;
            result = await _connection.ExecuteReaderAsync(sql, async reader =>
            {
                var messages = new List<OutboxMessage>();
                while (await reader.ReadAsync(cancellationToken))
                {
                    messages.Add(new OutboxMessage
                    {
                        OutboxName = reader.GetString(0),
                        Id = reader.GetGuid(1),
                        InstanceId = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Retries = reader.GetInt32(3),
                        SerializedMessage = JsonConvert.DeserializeObject<JsonSerializedMessage>(reader.GetString(4)),
                        Added = new DateTime(reader.GetInt64(5), DateTimeKind.Utc),
                    });
                }

                return messages;
            }, _transaction, sqlParams, cancellationToken);

            return result;
        }

        public async Task RemoveAllCompletedMessages(List<OutboxMessage> completedMessages, CancellationToken cancellationToken = default)
        {
            if (!completedMessages.Any()) return;

            using var cmd = _connection.CreateCommand();

            var sql = _statementProvider.RemoveAllCompletedMessagesStatement();

            var sqlParams = new List<object>();

            sqlParams.Add(cmd.CreateParameter("@OutboxName", completedMessages.First().OutboxName));

            var idParams = cmd.CreateArrayParameters(ref sql, "Id", completedMessages, DbType.Guid);
            sqlParams.AddRange(idParams);

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams, cancellationToken);
        }

        public async Task RemoveAllMessages(string outboxName, string instanceId, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", outboxName),
                cmd.CreateParameter("@Instance", instanceId),
            };

            var sql = _statementProvider.RemoveAllMessagesStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams, cancellationToken);
        }

        public async Task RemoveMessage(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", message.OutboxName),
                cmd.CreateParameter("@Id", message.Id),
            };

            var sql = _statementProvider.RemoveMessageStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams, cancellationToken);
        }

        public async Task ReserveMessages(IEnumerable<Guid> enumerable, string outboxName, string instanceId, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();

            var sql = _statementProvider.ReserveMessagesStatement();

            var sqlParams = new List<object>();

            sqlParams.Add(cmd.CreateParameter("@OutboxName", outboxName));
            sqlParams.Add(cmd.CreateParameter("@InstanceId", instanceId));

            var idParams = cmd.CreateArrayParameters(ref sql, "Id", enumerable, DbType.Guid);
            sqlParams.AddRange(idParams);

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams, cancellationToken);
        }
    }
}
