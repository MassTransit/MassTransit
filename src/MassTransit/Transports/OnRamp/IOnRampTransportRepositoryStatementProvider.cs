namespace MassTransit.Transports.OnRamp
{
    public interface IOnRampTransportRepositoryStatementProvider
    {
        string InsertMessageStatement();
    }
}
