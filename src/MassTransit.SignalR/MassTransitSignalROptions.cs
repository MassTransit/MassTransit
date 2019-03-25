namespace MassTransit.SignalR
{
    public class MassTransitSignalROptions
    {
        /// <summary>
        /// Gets or Sets whether or not to use MessageData for the Built In SignalR Consumers. Defaults to false.
        /// </summary>
        public bool UseMessageData { get; set; } = false;
    }
}
