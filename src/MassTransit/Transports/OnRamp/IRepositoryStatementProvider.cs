namespace MassTransit.Transports.Outbox
{
    public interface IRepositoryStatementProvider : IOnRampTransportRepositoryStatementProvider
        , ISweeperRepositoryStatementProvider
        , IClusterRepositoryStatementProvider
        , ILockRepositoryStatementProvider
    {
    }
}
