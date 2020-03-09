namespace MassTransit.Contracts
{
    /// <summary>
    /// Execute Activity description
    /// </summary>
    public interface ExecuteActivityInfo
    {
        string Name { get; }

        string ActivityType { get; }

        ArgumentInfo Argument { get; }

        VariableInfo[] OutputVariables { get; }
    }
}
