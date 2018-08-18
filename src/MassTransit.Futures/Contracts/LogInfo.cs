namespace MassTransit.Contracts
{
    /// <summary>
    /// Describes the log output of a routing slip activity
    /// </summary>
    public interface LogInfo
    {
        string LogType { get; }

        PropertyInfo[] Properties { get; }
    }
}