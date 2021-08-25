using MassTransit.Transports.Outbox.Configuration;

namespace MassTransit.Transports.Outbox.StatementProviders
{
    public class SqlServerRepositoryStatementProvider
        : SqlFormatItemRepositoryStatementProvider
        , IRepositoryStatementProvider
    {

        public SqlServerRepositoryStatementProvider(IOnRampTransportOptions onRampTransportOptions, IRepositoryNamingProvider repositoryNamingProvider)
            : base(onRampTransportOptions, repositoryNamingProvider)
        {
        }

        public override string FailedToSendMessagesStatement() => string.Format(base.FailedToSendMessagesStatement(), "@OnRampName", "@InstanceId");

        public override string FailedToSendMessageStatement() => string.Format(base.FailedToSendMessageStatement(), "@OnRampName", "@Id");

        public override string FetchNextMessagesStatement() => string.Format(base.FetchNextMessagesStatement(), "@OnRampName");

        public override string FreeAllMessagesFromAnySweeperInstanceStatement() => string.Format(base.FreeAllMessagesFromAnySweeperInstanceStatement(), "@OnRampName");

        public override string FreeMessagesFromFailedSweeperInstanceStatement() => string.Format(base.FreeMessagesFromFailedSweeperInstanceStatement(), "@OnRampName", "@InstanceId");

        public override string GetAllSweepersStatement() => string.Format(base.GetAllSweepersStatement(), "@OnRampName");

        public override string GetMessageSweeperInstanceIdsStatement() => string.Format(base.GetMessageSweeperInstanceIdsStatement(), "@OnRampName");

        public override string InsertLockStatement() => string.Format(base.InsertLockStatement(), "@OnRampName", "@LockName");

        public override string InsertMessageStatement() => string.Format(base.InsertMessageStatement(), "@OnRampName", "@Id", "@InstanceId", "@Retries", "@SerializedMessage", "@Added", "@ExpirationTime");

        public override string InsertSweeperStatement() => string.Format(base.InsertSweeperStatement(), "@OnRampName", "@InstanceId", "@LastCheckinTime", "@CheckinInterval");

        public override string RemoveAllCompletedMessagesStatement() => string.Format(base.RemoveAllCompletedMessagesStatement(), "@OnRampName", "{Id}");

        public override string RemoveAllMessagesStatement() => string.Format(base.RemoveAllMessagesStatement(), "@OnRampName", "@InstanceId");

        public override string RemoveMessageStatement() => string.Format(base.RemoveMessageStatement(), "@OnRampName", "@Id");

        public override string RemoveSweeperStatement() => string.Format(base.RemoveSweeperStatement(), "@OnRampName", "@InstanceId");

        public override string ReserveMessagesStatement() => string.Format(base.ReserveMessagesStatement(), "@InstanceId", "@OnRampName", "{Id}");

        public override string SelectRowLockStatement() => string.Format(base.SelectRowLockStatement(), "@OnRampName", "@LockName");

        public override string UpdateSweeperStatement() => string.Format(base.UpdateSweeperStatement(), "@LastCheckinTime", "@OnRampName", "@InstanceId");
    }
}
