using MassTransit.Context;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox.Repositories
{
    public class SqlServerRepositoryInitializer : IRepositoryInitializer
    {
        public SqlServerRepositoryInitializer(DbConnection connection, IRepositoryNamingProvider repositoryNamingProvider)
        {
            _connection = connection;
            _repositoryNamingProvider = repositoryNamingProvider;
        }

        private readonly DbConnection _connection;
        private readonly IRepositoryNamingProvider _repositoryNamingProvider;

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var sql =
                $@"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{_repositoryNamingProvider.Schema}')
BEGIN
	EXEC('CREATE SCHEMA [{_repositoryNamingProvider.Schema}]')
END;

IF OBJECT_ID(N'{_repositoryNamingProvider.GetLocksTableName()}',N'U') IS NULL
BEGIN
CREATE TABLE {_repositoryNamingProvider.GetLocksTableName()}(
	[OutboxName] [nvarchar](120) NOT NULL,
	[LockName] [nvarchar](40) NOT NULL,
 CONSTRAINT [PK_{_repositoryNamingProvider.GetLocksTableName()}] PRIMARY KEY CLUSTERED 
(
	[OutboxName] ASC,
	[LockName] ASC
))
END;

IF OBJECT_ID(N'{_repositoryNamingProvider.GetMessagesTableName()}',N'U') IS NULL
BEGIN
CREATE TABLE {_repositoryNamingProvider.GetMessagesTableName()}(
	[OutboxName] [nvarchar](120) NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
	[InstanceId] [nvarchar](100) NULL,
	[Retries] [int] NOT NULL,
	[SerializedMessage] [nvarchar](max) NOT NULL,
	[Added] [bigint] NOT NULL,
 CONSTRAINT [PK_{_repositoryNamingProvider.GetMessagesTableName()}] PRIMARY KEY CLUSTERED 
(
	[OutboxName] ASC,
	[Id] ASC
))
END;

IF OBJECT_ID(N'{_repositoryNamingProvider.GetSweepersTableName()}',N'U') IS NULL
BEGIN
CREATE TABLE {_repositoryNamingProvider.GetSweepersTableName()}(
	[OutboxName] [nvarchar](120) NOT NULL,
	[InstanceId] [nvarchar](100) NOT NULL,
	[LastCheckinTime] [bigint] NOT NULL,
	[CheckinInterval] [bigint] NOT NULL,
 CONSTRAINT [PK_{_repositoryNamingProvider.GetSweepersTableName()}] PRIMARY KEY CLUSTERED 
(
	[OutboxName] ASC,
	[InstanceId] ASC
))
END;";
            await _connection.ExecuteNonQueryAsync(sql);

            LogContext.Debug?.Log("Ensuring all create database tables script are applied.");
        }
    }
}
