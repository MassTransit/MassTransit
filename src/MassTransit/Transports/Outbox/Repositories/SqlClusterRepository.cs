using MassTransit.Transports.Outbox.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox.Repositories
{
    public class SqlClusterRepository : SqlLockRepository, IClusterRepository
    {
        private readonly IClusterRepositoryStatementProvider _statementProvider;

        public SqlClusterRepository(
            DbConnection dbConnection,
            IClusterRepositoryStatementProvider statementProvider,
            ILockRepositoryStatementProvider lockRepositoryStatementProvider)
            : base(dbConnection, lockRepositoryStatementProvider)
        {
            _statementProvider = statementProvider;
        }

        public async Task FreeAllMessagesFromAnySweeperInstance(string outboxName, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", outboxName),
            };

            var sql = _statementProvider.FreeAllMessagesFromAnySweeperInstanceStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);
        }

        public async Task FreeMessagesFromFailedSweeperInstance(string outboxName, string instanceId, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", outboxName),
                cmd.CreateParameter("@InstanceId", instanceId),
            };

            var sql = _statementProvider.FreeMessagesFromFailedSweeperInstanceStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);
        }

        public async Task<IReadOnlyList<OutboxSweeper>> GetAllSweepers(string outboxName, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", outboxName),
            };

            var sql = _statementProvider.GetAllSweepersStatement();

            List<OutboxSweeper> result;
            result = await _connection.ExecuteReaderAsync(sql, async reader =>
            {
                var messages = new List<OutboxSweeper>();
                while (await reader.ReadAsync())
                {
                    messages.Add(new OutboxSweeper
                    {
                        OutboxName = reader.GetString(0),
                        InstanceId = reader.GetString(1),
                        LastCheckinTime = new DateTime(reader.GetInt64(2), DateTimeKind.Utc),
                        CheckinInterval = TimeSpan.FromMilliseconds(reader.GetInt64(3)),
                    });
                }

                return messages;
            }, _transaction, sqlParams);

            return result;
        }

        public async Task<IReadOnlyList<string>> GetMessageSweeperInstanceIds(string outboxName, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", outboxName),
            };

            var sql = _statementProvider.GetMessageSweeperInstanceIdsStatement();

            List<string> result;
            result = await _connection.ExecuteReaderAsync(sql, async reader =>
            {
                var messages = new List<string>();
                while (await reader.ReadAsync())
                {
                    messages.Add(reader.GetString(0));
                }

                return messages;
            }, _transaction, sqlParams);

            return result;
        }

        public async Task InsertSweeper(string outboxName, string instanceId, DateTime lastCheckin, TimeSpan checkinInterval, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", outboxName),
                cmd.CreateParameter("@InstanceId", instanceId),
                cmd.CreateParameter("@LastCheckinTime", lastCheckin.Ticks),
                cmd.CreateParameter("@CheckinInterval", Convert.ToInt64(checkinInterval.TotalMilliseconds)),
            };

            var sql = _statementProvider.InsertSweeperStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);
        }

        public async Task RemoveSweeper(string outboxName, string instanceId, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", outboxName),
                cmd.CreateParameter("@InstanceId", instanceId),
            };

            var sql = _statementProvider.RemoveSweeperStatement();

            await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);
        }

        public async Task<bool> UpdateSweeper(string outboxName, string instanceId, DateTime lastCheckin, CancellationToken cancellationToken = default)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OutboxName", outboxName),
                cmd.CreateParameter("@InstanceId", instanceId),
                cmd.CreateParameter("@LastCheckinTime", lastCheckin.Ticks),
            };

            var sql = _statementProvider.UpdateSweeperStatement();

            var result = await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);

            return result == 1;
        }
    }
}
