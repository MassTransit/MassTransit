namespace MassTransit.Contracts
{
    /// <summary>
    /// A contract represents a message, activity arguments, anything that can be sent across the wire
    /// </summary>
    public interface MessageInfo :
        ObjectInfo
    {
        /// <summary>
        /// Message types supported by this message
        /// </summary>
        string[] MessageTypes { get; }
    }
}
