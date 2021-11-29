namespace MassTransit.Scheduling
{
    /// <summary>
    /// If the scheduler is offline and comes back online, the policy determines how
    /// a missed scheduled message is handled.
    /// </summary>
    public enum MissedEventPolicy
    {
        /// <summary>
        /// use the default handling of the scheduler
        /// </summary>
        Default,

        /// <summary>
        /// Skip the event, waiting for the next scheduled interval
        /// </summary>
        Skip,

        /// <summary>
        /// Send the message immediately and then continue the schedule as planned
        /// </summary>
        Send
    }
}
