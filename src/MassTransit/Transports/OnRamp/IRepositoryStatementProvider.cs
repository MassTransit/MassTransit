namespace MassTransit.Transports.OnRamp
{
    public interface IRepositoryStatementProvider : IOnRampTransportRepositoryStatementProvider
        , ISweeperRepositoryStatementProvider
        , IClusterRepositoryStatementProvider
        , ILockRepositoryStatementProvider
    {
    }
}
