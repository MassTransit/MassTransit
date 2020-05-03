namespace MassTransit.SignalR
{
    using System;


    [Obsolete]
    public class MassTransitSignalROptions
    {
        [Obsolete("Use HubLifetimeManagerOptions.UseMessageData")]
        /// <summary>
        /// Gets or Sets whether or not to use MessageData for the Built In SignalR Consumers. Defaults to false.
        /// </summary>
        public bool UseMessageData { get; set; } = false;
    }
}
