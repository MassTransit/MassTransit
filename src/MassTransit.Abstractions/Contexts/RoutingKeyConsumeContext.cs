namespace MassTransit
{
    public interface RoutingKeyConsumeContext
    {
        /// <summary>
        /// The routing key for the message (defaults to "")
        /// </summary>
        string? RoutingKey { get; }
    }
}
