namespace MassTransit.Contracts.Metadata
{
    /// <summary>
    /// Activity description
    /// </summary>
    public interface ActivityInfo :
        ExecuteActivityInfo
    {
        LogInfo Log { get; }
    }
}
