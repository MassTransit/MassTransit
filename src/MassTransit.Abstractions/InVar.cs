namespace MassTransit
{
    using Initializers.Variables;


    /// <summary>
    /// Variables, which can be used for message initialization
    /// </summary>
    public static class InVar
    {
        /// <summary>
        /// Generates the current timestamp, in UTC, which can be used to initialize properties
        /// in the message with a consistent value
        /// </summary>
        public static TimestampVariable Timestamp => new TimestampVariable();

        /// <summary>
        /// Generates a new identifier, and maintains that identifier for the entire message initializer lifetime,
        /// so that subsequent uses of the identifier return the same value. There are multiple aliases for the same
        /// identifier, so that property names are automatically inferred (Id, CorrelationId, etc.).
        /// </summary>
        public static IdVariable Id => new IdVariable();

        /// <summary>
        /// Generates a new identifier, and maintains that identifier for the entire message initializer lifetime,
        /// so that subsequent uses of the identifier return the same value. There are multiple aliases for the same
        /// identifier, so that property names are automatically inferred (Id, CorrelationId, etc.).
        /// </summary>
        public static IdVariable CorrelationId => new IdVariable();
    }
}
