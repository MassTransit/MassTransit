namespace MassTransit.Contracts
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
