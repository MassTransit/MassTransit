namespace MassTransit.Contracts
{
    /// <summary>
    /// Describes a consumer
    /// </summary>
    public interface ConsumerInfo
    {
        /// <summary>
        /// The shortened consumer type
        /// </summary>
        string ConsumerType { get; }

        /// <summary>
        /// The message types consumed by the this consumer
        /// </summary>
        MessageInfo[] Messages { get; }
    }
}
