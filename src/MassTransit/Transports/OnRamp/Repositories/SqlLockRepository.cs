using System.Data.Common;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox.Repositories
{
    public class SqlLockRepository : ConnectionAndTransactionHolder, ILockRepository
    {
        private readonly ILockRepositoryStatementProvider _statementProvider;

        public SqlLockRepository(DbConnection dbConnection, ILockRepositoryStatementProvider statementProvider)
            : base(dbConnection)
        {
            _statementProvider = statementProvider;
        }

        public async Task ObtainLock(string onRampName, string lockName)
        {
            using var cmd = _connection.CreateCommand();
            object[] sqlParams =
            {
                cmd.CreateParameter("@OnRampName", onRampName),
                cmd.CreateParameter("@LockName", lockName),
            };

            var sql = _statementProvider.SelectRowLockStatement();

            if(!await _connection.ExecuteReaderAsync(sql, reader => reader.ReadAsync(), _transaction, sqlParams))
            {
                // Didn't find lock, so we must create it
                sql = _statementProvider.InsertLockStatement();

                sqlParams = new object[]
                {
                    cmd.CreateParameter("@OnRampName", onRampName),
                    cmd.CreateParameter("@LockName", lockName),
                };

                await _connection.ExecuteNonQueryAsync(sql, _transaction, sqlParams);
            }
        }
    }
}
