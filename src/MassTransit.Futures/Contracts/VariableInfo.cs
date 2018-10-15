namespace MassTransit.Contracts
{
    /// <summary>
    /// Describes a variable added to a routing slip activity
    /// </summary>
    public interface VariableInfo
    {
        string Name { get; }

        string VariableType { get; }

        PropertyInfo[] Properties { get; }
    }
}