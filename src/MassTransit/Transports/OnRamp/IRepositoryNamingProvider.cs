namespace MassTransit.Transports.OnRamp
{
    public interface IRepositoryNamingProvider
    {
        string Schema { get; }
        string FormatColumnName(string columnName);
        string GetLocksTableName();
        string GetMessagesTableName();
        string GetSweepersTableName();
        string GetInitializationScript();
    }
}
