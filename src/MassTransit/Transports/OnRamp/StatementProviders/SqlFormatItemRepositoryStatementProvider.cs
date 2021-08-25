using MassTransit.Transports.Outbox.Configuration;

namespace MassTransit.Transports.Outbox.StatementProviders
{
    public class SqlFormatItemRepositoryStatementProvider
        : IRepositoryStatementProvider
    {
        private readonly IOnRampTransportOptions _onRampTransportOptions;
        private readonly IRepositoryNamingProvider _repositoryNamingProvider;

        public SqlFormatItemRepositoryStatementProvider(IOnRampTransportOptions onRampTransportOptions, IRepositoryNamingProvider repositoryNamingProvider)
        {
            _onRampTransportOptions = onRampTransportOptions;
            _repositoryNamingProvider = repositoryNamingProvider;
        }

        public virtual string FailedToSendMessagesStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET [Retries] = [Retries] + 1 WHERE [OnRampName] = {{0}} AND [InstanceId] = {{1}}";
            return sql;
        }

        public virtual string FailedToSendMessageStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET [Retries] = [Retries] + 1 WHERE [OnRampName] = {{0}} AND [Id] = {{1}}";

            return sql;
        }

        public virtual string FetchNextMessagesStatement()
        {
            var sql = $"SELECT TOP ({_onRampTransportOptions.PrefetchCount}) [OnRampName],[Id],[InstanceId],[Retries],[SerializedMessage],[Added],[ExpirationTime] " +
                $"FROM {_repositoryNamingProvider.GetMessagesTableName()} WITH (readpast) WHERE [OnRampName] = @OnRampName AND [InstanceId] IS NULL AND [Retries] < {_onRampTransportOptions.SendAttemptThreshold}" +
                $"ORDER BY [Added]";

            return sql;
        }

        public virtual string FreeAllMessagesFromAnySweeperInstanceStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET [InstanceId] = NULL WHERE [OnRampName] = {{0}} AND [InstanceId] IS NOT NULL";
            return sql;
        }

        public virtual string FreeMessagesFromFailedSweeperInstanceStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET [InstanceId] = NULL WHERE [OnRampName] = {{0}} AND [InstanceId] = {{1}}";
            return sql;
        }

        public virtual string GetAllSweepersStatement()
        {
            var sql = $"SELECT [OnRampName],[InstanceId],[LastCheckinTime],[CheckinInterval] FROM {_repositoryNamingProvider.GetSweepersTableName()} WHERE [OnRampName] = {{0}}";
            return sql;
        }

        public virtual string GetMessageSweeperInstanceIdsStatement()
        {
            var sql = $"SELECT DISTINCT [InstanceId] FROM {_repositoryNamingProvider.GetMessagesTableName()} WHERE [OnRampName] = {{0}} AND [InstanceId] IS NOT NULL";
            return sql;
        }

        public virtual string InsertLockStatement()
        {
            var sql =
                $"INSERT INTO {_repositoryNamingProvider.GetLocksTableName()}([OnRampName],[LockName])" +
                $"VALUES({{0}},{{1}});";
            return sql;
        }

        public virtual string InsertMessageStatement()
        {
            var sql =
                $"INSERT INTO {_repositoryNamingProvider.GetMessagesTableName()}([OnRampName],[Id],[InstanceId],[Retries],[SerializedMessage],[Added],[ExpirationTime])" +
                $"VALUES({{0}},{{1}},{{2}},{{3}},{{4}},{{5}},{{6}});";
            return sql;
        }

        public virtual string InsertSweeperStatement()
        {
            var sql =
                $"INSERT INTO {_repositoryNamingProvider.GetSweepersTableName()}([OnRampName],[InstanceId],[LastCheckinTime],[CheckinInterval])" +
                $"VALUES({{0}},{{1}},{{2}},{{3}});";

            return sql;
        }

        public virtual string RemoveAllCompletedMessagesStatement()
        {
            var sql =
                $"DELETE FROM {_repositoryNamingProvider.GetMessagesTableName()} WHERE [OnRampName] = {{0}} AND [Id] IN ({{1}})";
            return sql;
        }

        public virtual string RemoveAllMessagesStatement()
        {
            var sql =
                $"DELETE FROM {_repositoryNamingProvider.GetMessagesTableName()} WHERE [OnRampName] = {{0}} AND [InstanceId] = {{1}}";
            return sql;
        }

        public virtual string RemoveMessageStatement()
        {
            var sql = $"DELETE FROM {_repositoryNamingProvider.GetMessagesTableName()} WHERE [OnRampName] = {{0}} AND [Id] = {{1}}";
            return sql;
        }

        public virtual string RemoveSweeperStatement()
        {
            var sql = $"DELETE FROM {_repositoryNamingProvider.GetSweepersTableName()} WHERE [OnRampName] = {{0}} AND [InstanceId] = {{1}}";
            return sql;
        }

        public virtual string ReserveMessagesStatement()
        {
            var sql =
                $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET [InstanceId] = {{0}} WHERE [OnRampName] = {{1}} AND [Id] IN ({{2}})"; // or possible $@ https://stackoverflow.com/questions/31333096/how-to-use-escape-characters-with-string-interpolation-in-c-sharp-6
            return sql;
        }

        public virtual string SelectRowLockStatement()
        {
            var sql = $"SELECT * FROM {_repositoryNamingProvider.GetLocksTableName()} WITH (UPDLOCK,ROWLOCK) WHERE [OnRampName] = {{0}} AND [LockName] = {{1}}";
            return sql;
        }

        public virtual string UpdateSweeperStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetSweepersTableName()} SET [LastCheckinTime] = {{0}} WHERE [OnRampName] = {{1}} AND [InstanceId] = {{2}}";
            return sql;
        }
    }
}
