namespace MassTransit.Transports.OnRamp
{
    public interface IClusterRepositoryStatementProvider
    {
        // The cluster manager will need to have the ability to create a new transaction, so unlikely we can have using statement
        string FreeAllMessagesFromAnySweeperInstanceStatement();
        string FreeMessagesFromFailedSweeperInstanceStatement();
        string GetAllSweepersStatement();
        string GetMessageSweeperInstanceIdsStatement();
        string InsertSweeperStatement();
        string RemoveSweeperStatement();
        string UpdateSweeperStatement();
    }
}
