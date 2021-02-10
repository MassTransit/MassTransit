using MassTransit.Transports.Outbox.Configuration;

namespace MassTransit.Transports.Outbox.StatementProviders
{
    public class SqlServerRepositoryStatementProvider
        : SqlFormatItemRepositoryStatementProvider
        , IRepositoryStatementProvider
    {

        public SqlServerRepositoryStatementProvider(IOutboxTransportOptions outboxTransportOptions, IRepositoryNamingProvider repositoryNamingProvider)
            : base(outboxTransportOptions, repositoryNamingProvider)
        {
        }

        public override string FailedToSendMessagesStatement() => string.Format(base.FailedToSendMessagesStatement(), "@OutboxName", "@InstanceId");

        public override string FailedToSendMessageStatement() => string.Format(base.FailedToSendMessageStatement(), "@OutboxName", "@Id");

        public override string FetchNextMessagesStatement() => string.Format(base.FetchNextMessagesStatement(), "@OutboxName");

        public override string FreeAllMessagesFromAnySweeperInstanceStatement() => string.Format(base.FreeAllMessagesFromAnySweeperInstanceStatement(), "@OutboxName");

        public override string FreeMessagesFromFailedSweeperInstanceStatement() => string.Format(base.FreeMessagesFromFailedSweeperInstanceStatement(), "@OutboxName", "@InstanceId");

        public override string GetAllSweepersStatement() => string.Format(base.GetAllSweepersStatement(), "@OutboxName");

        public override string GetMessageSweeperInstanceIdsStatement() => string.Format(base.GetMessageSweeperInstanceIdsStatement(), "@OutboxName");

        public override string InsertLockStatement() => string.Format(base.InsertLockStatement(), "@OutboxName", "@LockName");

        public override string InsertMessageStatement() => string.Format(base.InsertMessageStatement(), "@OutboxName", "@Id", "@InstanceId", "@Retries", "@SerializedMessage", "@Added", "@ExpirationTime");

        public override string InsertSweeperStatement() => string.Format(base.InsertSweeperStatement(), "@OutboxName", "@InstanceId", "@LastCheckinTime", "@CheckinInterval");

        public override string RemoveAllCompletedMessagesStatement() => string.Format(base.RemoveAllCompletedMessagesStatement(), "@OutboxName", "{Id}");

        public override string RemoveAllMessagesStatement() => string.Format(base.RemoveAllMessagesStatement(), "@OutboxName", "@InstanceId");

        public override string RemoveMessageStatement() => string.Format(base.RemoveMessageStatement(), "@OutboxName", "@Id");

        public override string RemoveSweeperStatement() => string.Format(base.RemoveSweeperStatement(), "@OutboxName", "@InstanceId");

        public override string ReserveMessagesStatement() => string.Format(base.ReserveMessagesStatement(), "@InstanceId", "@OutboxName", "{Id}");

        public override string SelectRowLockStatement() => string.Format(base.SelectRowLockStatement(), "@OutboxName", "@LockName");

        public override string UpdateSweeperStatement() => string.Format(base.UpdateSweeperStatement(), "@LastCheckinTime", "@OutboxName", "@InstanceId");
    }
}
