namespace MassTransit.Contracts
{
    /// <summary>
    /// Describes the log output of a routing slip activity
    /// </summary>
    public interface LogInfo :
        ObjectInfo
    {
        string LogType { get; }
    }
}
