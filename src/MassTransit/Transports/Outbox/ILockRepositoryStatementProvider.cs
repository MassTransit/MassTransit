namespace MassTransit.Transports.Outbox
{
    public interface ILockRepositoryStatementProvider
    {
        string SelectRowLockStatement();
        string InsertLockStatement();
    }
}
