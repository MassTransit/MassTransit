namespace MassTransit.Transports.Outbox
{
    public interface IRepositoryNamingProvider
    {
        string Schema { get; }
        string GetLocksTableName();
        string GetMessagesTableName();
        string GetSweepersTableName();
    }
}
