namespace MassTransit.Transports.Outbox
{
    public interface IRepositoryStatementProvider : IOutboxTransportRepositoryStatementProvider
        , ISweeperRepositoryStatementProvider
        , IClusterRepositoryStatementProvider
        , ILockRepositoryStatementProvider
    {
    }
}
