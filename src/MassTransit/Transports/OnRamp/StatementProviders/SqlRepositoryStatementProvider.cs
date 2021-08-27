using MassTransit.Transports.OnRamp.Configuration;

namespace MassTransit.Transports.OnRamp.StatementProviders
{
    public class SqlRepositoryStatementProvider
        : IRepositoryStatementProvider
    {
        protected readonly IOnRampTransportOptions _onRampTransportOptions;
        protected readonly IRepositoryNamingProvider _repositoryNamingProvider;

        public SqlRepositoryStatementProvider(IOnRampTransportOptions onRampTransportOptions, IRepositoryNamingProvider repositoryNamingProvider)
        {
            _onRampTransportOptions = onRampTransportOptions;
            _repositoryNamingProvider = repositoryNamingProvider;
        }

        public virtual string FailedToSendMessagesStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET {_repositoryNamingProvider.FormatColumnName("Retries")} = {_repositoryNamingProvider.FormatColumnName("Retries")} + 1 WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("InstanceId")} = @InstanceId";
            return sql;
        }

        public virtual string FailedToSendMessageStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET {_repositoryNamingProvider.FormatColumnName("Retries")} = {_repositoryNamingProvider.FormatColumnName("Retries")} + 1 WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("Id")} = @Id";

            return sql;
        }

        public virtual string FetchNextMessagesStatement()
        {
            var sql = $"SELECT {_repositoryNamingProvider.FormatColumnName("OnRampName")},{_repositoryNamingProvider.FormatColumnName("Id")},{_repositoryNamingProvider.FormatColumnName("InstanceId")},{_repositoryNamingProvider.FormatColumnName("Retries")},{_repositoryNamingProvider.FormatColumnName("SerializedMessage")},{_repositoryNamingProvider.FormatColumnName("Added")} " +
                $"FROM {_repositoryNamingProvider.GetMessagesTableName()} WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("InstanceId")} IS NULL AND {_repositoryNamingProvider.FormatColumnName("Retries")} < {_onRampTransportOptions.SendAttemptThreshold} " +
                $"ORDER BY {_repositoryNamingProvider.FormatColumnName("Added")} " +
                $"LIMIT ({_onRampTransportOptions.PrefetchCount})";

            return sql;
        }

        public virtual string FreeAllMessagesFromAnySweeperInstanceStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET {_repositoryNamingProvider.FormatColumnName("InstanceId")} = NULL WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("InstanceId")} IS NOT NULL";
            return sql;
        }

        public virtual string FreeMessagesFromFailedSweeperInstanceStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET {_repositoryNamingProvider.FormatColumnName("InstanceId")} = NULL WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("InstanceId")} = @InstanceId";
            return sql;
        }

        public virtual string GetAllSweepersStatement()
        {
            var sql = $"SELECT {_repositoryNamingProvider.FormatColumnName("OnRampName")},{_repositoryNamingProvider.FormatColumnName("InstanceId")},{_repositoryNamingProvider.FormatColumnName("LastCheckinTime")},{_repositoryNamingProvider.FormatColumnName("CheckinInterval")} FROM {_repositoryNamingProvider.GetSweepersTableName()} WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName";
            return sql;
        }

        public virtual string GetMessageSweeperInstanceIdsStatement()
        {
            var sql = $"SELECT DISTINCT {_repositoryNamingProvider.FormatColumnName("InstanceId")} FROM {_repositoryNamingProvider.GetMessagesTableName()} WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("InstanceId")} IS NOT NULL";
            return sql;
        }

        public virtual string InsertLockStatement()
        {
            var sql =
                $"INSERT INTO {_repositoryNamingProvider.GetLocksTableName()}({_repositoryNamingProvider.FormatColumnName("OnRampName")},{_repositoryNamingProvider.FormatColumnName("LockName")})" +
                $"VALUES(@OnRampName,@LockName);";
            return sql;
        }

        public virtual string InsertMessageStatement()
        {
            var sql =
                $"INSERT INTO {_repositoryNamingProvider.GetMessagesTableName()}({_repositoryNamingProvider.FormatColumnName("OnRampName")},{_repositoryNamingProvider.FormatColumnName("Id")},{_repositoryNamingProvider.FormatColumnName("InstanceId")},{_repositoryNamingProvider.FormatColumnName("Retries")},{_repositoryNamingProvider.FormatColumnName("SerializedMessage")},{_repositoryNamingProvider.FormatColumnName("Added")})" +
                $"VALUES(@OnRampName,@Id,@InstanceId,@Retries,@SerializedMessage,@Added);";
            return sql;
        }

        public virtual string InsertSweeperStatement()
        {
            var sql =
                $"INSERT INTO {_repositoryNamingProvider.GetSweepersTableName()}({_repositoryNamingProvider.FormatColumnName("OnRampName")},{_repositoryNamingProvider.FormatColumnName("InstanceId")},{_repositoryNamingProvider.FormatColumnName("LastCheckinTime")},{_repositoryNamingProvider.FormatColumnName("CheckinInterval")})" +
                $"VALUES(@OnRampName,@InstanceId,@LastCheckinTime,@CheckinInterval);";

            return sql;
        }

        public virtual string RemoveAllCompletedMessagesStatement()
        {
            var sql =
                $"DELETE FROM {_repositoryNamingProvider.GetMessagesTableName()} WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("Id")} IN ({{Id}})";
            return sql;
        }

        public virtual string RemoveAllMessagesStatement()
        {
            var sql =
                $"DELETE FROM {_repositoryNamingProvider.GetMessagesTableName()} WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("InstanceId")} = @InstanceId";
            return sql;
        }

        public virtual string RemoveMessageStatement()
        {
            var sql = $"DELETE FROM {_repositoryNamingProvider.GetMessagesTableName()} WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("Id")} = @Id";
            return sql;
        }

        public virtual string RemoveSweeperStatement()
        {
            var sql = $"DELETE FROM {_repositoryNamingProvider.GetSweepersTableName()} WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("InstanceId")} = @InstanceId";
            return sql;
        }

        public virtual string ReserveMessagesStatement()
        {
            var sql =
                $"UPDATE {_repositoryNamingProvider.GetMessagesTableName()} SET {_repositoryNamingProvider.FormatColumnName("InstanceId")} = @InstanceId WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("Id")} IN ({{Id}})"; // or possible $@ https://stackoverflow.com/questions/31333096/how-to-use-escape-characters-with-string-interpolation-in-c-sharp-6
            return sql;
        }

        public virtual string SelectRowLockStatement()
        {
            var sql = $"SELECT * FROM {_repositoryNamingProvider.GetLocksTableName()} WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("LockName")} = @LockName";
            return sql;
        }

        public virtual string UpdateSweeperStatement()
        {
            var sql = $"UPDATE {_repositoryNamingProvider.GetSweepersTableName()} SET {_repositoryNamingProvider.FormatColumnName("LastCheckinTime")} = @LastCheckinTime WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("InstanceId")} = @InstanceId";
            return sql;
        }
    }
}
