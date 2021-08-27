namespace MassTransit.Transports.OnRamp
{
    public interface ILockRepositoryStatementProvider
    {
        string SelectRowLockStatement();
        string InsertLockStatement();
    }
}
