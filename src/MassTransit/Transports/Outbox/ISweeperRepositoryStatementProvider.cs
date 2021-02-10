namespace MassTransit.Transports.Outbox
{
    public interface ISweeperRepositoryStatementProvider
    {
        string FailedToSendMessageStatement();
        string FetchNextMessagesStatement();
        string RemoveMessageStatement();
        string ReserveMessagesStatement();
        string RemoveAllMessagesStatement();
        string RemoveAllCompletedMessagesStatement();
        string FailedToSendMessagesStatement();
    }
}
