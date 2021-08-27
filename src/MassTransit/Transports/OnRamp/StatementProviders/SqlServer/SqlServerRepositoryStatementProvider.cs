using MassTransit.Transports.OnRamp.Configuration;

namespace MassTransit.Transports.OnRamp.StatementProviders
{
    public class SqlServerRepositoryStatementProvider
        : SqlRepositoryStatementProvider
    {

        public SqlServerRepositoryStatementProvider(IOnRampTransportOptions onRampTransportOptions, IRepositoryNamingProvider repositoryNamingProvider)
            : base(onRampTransportOptions, repositoryNamingProvider)
        {
        }

        public override string FetchNextMessagesStatement()
        {
            var sql = $"SELECT TOP ({_onRampTransportOptions.PrefetchCount}) {_repositoryNamingProvider.FormatColumnName("OnRampName")},{_repositoryNamingProvider.FormatColumnName("Id")},{_repositoryNamingProvider.FormatColumnName("InstanceId")},{_repositoryNamingProvider.FormatColumnName("Retries")},{_repositoryNamingProvider.FormatColumnName("SerializedMessage")},{_repositoryNamingProvider.FormatColumnName("Added")} " +
                $"FROM {_repositoryNamingProvider.GetMessagesTableName()} WITH (readpast) WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("InstanceId")} IS NULL AND {_repositoryNamingProvider.FormatColumnName("Retries")} < {_onRampTransportOptions.SendAttemptThreshold} " +
                $"ORDER BY {_repositoryNamingProvider.FormatColumnName("Added")}";

            return sql;
        }

        public override string SelectRowLockStatement()
        {
            var sql = $"SELECT * FROM {_repositoryNamingProvider.GetLocksTableName()} WITH (UPDLOCK,ROWLOCK) WHERE {_repositoryNamingProvider.FormatColumnName("OnRampName")} = @OnRampName AND {_repositoryNamingProvider.FormatColumnName("LockName")} = @LockName";
            return sql;
        }
    }
}
