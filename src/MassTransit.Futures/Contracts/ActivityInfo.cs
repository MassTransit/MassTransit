namespace MassTransit.Contracts
{
    /// <summary>
    /// Describes an activity
    /// </summary>
    public interface ActivityInfo
    {
        string Name { get; }

        string ActivityType { get; }

        ArgumentInfo Argument { get; }

        LogInfo Log { get; }

        VariableInfo[] OutputVariables { get; }
    }
}