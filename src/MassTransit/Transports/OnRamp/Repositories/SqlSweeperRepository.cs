using MassTransit.Transports.OnRamp.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp.Repositories
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

        public async Task FailedToSendMessage(OnRampSweeperSendException exception, CancellationToken cancellationToken = default)
        {
            // Do I want to log these exceptions?
            var message = exception.OnRampMessage;

            var sql = _statementProvider.FailedToSendMessageStatement();

            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OnRampName", message.OnRampName),
                cmd.CreateParameter("@Id", message.Id),
            };

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);
        }

        public async Task FailedToSendMessages(List<OnRampMessage> failedMessages, string instanceId, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OnRampName", failedMessages[0].OnRampName),
                cmd.CreateParameter("@InstanceId", instanceId),
            };

            var sql = _statementProvider.FailedToSendMessagesStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);
        }

        public async Task<IReadOnlyList<OnRampMessage>> FetchNextMessages(string onRampName, int prefetchCount, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();

            var p = cmd.CreateParameter("@OnRampName", onRampName);
            object[] sqlParams =
            {
                p
            };

            var sql = _statementProvider.FetchNextMessagesStatement();

            List<OnRampMessage> result;
            result = await _connection.ExecuteReaderAsync(sql, async reader =>
            {
                var messages = new List<OnRampMessage>();
                while (await reader.ReadAsync(cancellationToken))
                {
                    messages.Add(new OnRampMessage
                    {
                        OnRampName = reader.GetString(0),
                        Id = reader.GetGuid(1),
                        InstanceId = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Retries = reader.GetInt32(3),
                        SerializedMessage = JsonConvert.DeserializeObject<OnRampSerializedMessage>(reader.GetString(4)),
                        Added = new DateTime(reader.GetInt64(5), DateTimeKind.Utc),
                    });
                }

                return messages;
            }, _transaction, sqlParams, cancellationToken);

            return result;
        }

        public async Task RemoveAllCompletedMessages(List<OnRampMessage> completedMessages, CancellationToken cancellationToken = default)
        {
            if (!completedMessages.Any()) return;

            using var cmd = _connection.CreateCommand();

            var sql = _statementProvider.RemoveAllCompletedMessagesStatement();

            var sqlParams = new List<object>();

            sqlParams.Add(cmd.CreateParameter("@OnRampName", completedMessages.First().OnRampName));

            var idParams = cmd.CreateArrayParameters(ref sql, "Id", completedMessages, DbType.Guid);
            sqlParams.AddRange(idParams);

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams, cancellationToken);
        }

        public async Task RemoveAllMessages(string onRampName, string instanceId, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OnRampName", onRampName),
                cmd.CreateParameter("@InstanceId", instanceId),
            };

            var sql = _statementProvider.RemoveAllMessagesStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams, cancellationToken);
        }

        public async Task RemoveMessage(OnRampMessage message, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OnRampName", message.OnRampName),
                cmd.CreateParameter("@Id", message.Id),
            };

            var sql = _statementProvider.RemoveMessageStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams, cancellationToken);
        }

        public async Task ReserveMessages(IEnumerable<Guid> enumerable, string onRampName, string instanceId, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();

            var sql = _statementProvider.ReserveMessagesStatement();

            var sqlParams = new List<object>();

            sqlParams.Add(cmd.CreateParameter("@OnRampName", onRampName));
            sqlParams.Add(cmd.CreateParameter("@InstanceId", instanceId));

            var idParams = cmd.CreateArrayParameters(ref sql, "Id", enumerable, DbType.Guid);
            sqlParams.AddRange(idParams);

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams, cancellationToken);
        }
    }
}
