namespace MassTransit.Transports.Outbox
{
    public interface IOnRampTransportRepositoryStatementProvider
    {
        string InsertMessageStatement();
    }
}
