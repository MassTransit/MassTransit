namespace MassTransit.Contracts
{
    /// <summary>
    /// An object that is part of a message contract, etc.
    /// </summary>
    public interface ObjectInfo
    {
        /// <summary>
        /// Message types supported by this message
        /// </summary>
        string ObjectType { get; }

        /// <summary>
        /// Properties supported by this message
        /// </summary>
        PropertyInfo[] Properties { get; }
    }
}
