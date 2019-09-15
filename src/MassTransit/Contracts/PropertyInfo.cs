namespace MassTransit.Contracts
{
    /// <summary>
    /// Describes a property of any other info
    /// </summary>
    public interface PropertyInfo
    {
        /// <summary>
        /// The name of the property within the message
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The kind of property, value, object, list, or dictionary
        /// </summary>
        PropertyKind Kind { get; }

        /// <summary>
        /// The property type,
        /// </summary>
        string PropertyType { get; }

        /// <summary>
        /// If the layout is a dictionary, the key type for the dictionary
        /// </summary>
        string KeyType { get; }
    }
}
