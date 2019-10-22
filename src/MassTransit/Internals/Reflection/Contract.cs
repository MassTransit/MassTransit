namespace MassTransit.Internals.Reflection
{
    /// <summary>
    /// A contract represents a message type, which can include messages, routing slip activity arguments and logs, or
    /// really anything that is part of the messaging system.
    /// </summary>
    public class Contract
    {
        public Contract(string name, Property[] properties)
        {
            Name = name;
            Properties = properties;
        }

        /// <summary>
        /// The full message urn for the message type
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The properties of the message contract
        /// </summary>
        public Property[] Properties { get; }
    }
}
