namespace MassTransit
{
    using System;


    /// <summary>
    /// Customize the redelivery experience
    /// </summary>
    [Flags]
    public enum RedeliveryOptions
    {
        None = 0,

        /// <summary>
        /// Generate a new MessageId for the redelivered message (typically to avoid
        /// broker deduplication logic)
        /// </summary>
        ReplaceMessageId = 1,

        /// <summary>
        /// If specified, use the message scheduler context instead of the redelivery context (only use when transport-level redelivery is not available)
        /// </summary>
        UseMessageScheduler = 2,
    }
}
