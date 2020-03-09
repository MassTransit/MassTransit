namespace MassTransit.Contracts
{
    /// <summary>
    /// A node can describe the capabilities supported, including client-routed messages, rate limiting, authentication, ask allocation
    /// </summary>
    public interface ServiceCapability
    {
        /// <summary>
        /// Type name of the capability
        /// </summary>
        string CapabilityType { get; }
    }
}
