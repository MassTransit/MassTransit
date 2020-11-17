namespace MassTransit.Transports.Outbox
{
    public interface IOutboxTransportRepositoryStatementProvider
    {
        string InsertMessageStatement();
    }
}
