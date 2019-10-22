namespace MassTransit.Contracts
{
    /// <summary>
    /// Describes the argument of a routing slip activity
    /// </summary>
    public interface ArgumentInfo
    {
        string ArgumentType { get; }

        PropertyInfo[] Properties { get; }
    }
}
