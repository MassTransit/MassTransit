namespace MassTransit
{
    using System;


    /// <summary>
    /// If present in the container, these options will be used by the MassTransit hosted service.
    /// </summary>
    public class MassTransitHostOptions
    {
        /// <summary>
        /// If True, the hosted service will not return from StartAsync until the bus has started.
        /// </summary>
        public bool WaitUntilStarted { get; set; }

        /// <summary>
        /// If specified, the timeout will be used with StartAsync to cancel if the timeout is reached
        /// </summary>
        public TimeSpan? StartTimeout { get; set; }

        /// <summary>
        /// If specified, the timeout will be used with StopAsync to cancel if the timeout is reached.
        /// The bus is still stopped, only the wait is canceled.
        /// </summary>
        public TimeSpan? StopTimeout { get; set; }
    }
}
