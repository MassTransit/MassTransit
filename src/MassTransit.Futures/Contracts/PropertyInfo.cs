namespace MassTransit.Contracts
{
    /// <summary>
    /// Describes a property of any other info
    /// </summary>
    public interface PropertyInfo
    {
        string Name { get; }

        string PropertyType { get; }
    }
}