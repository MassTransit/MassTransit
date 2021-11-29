namespace MassTransit
{
    /// <summary>
    /// The reason this batch was made ready for consumption
    /// </summary>
    public enum BatchCompletionMode
    {
        /// <summary>
        /// The time limit for receiving messages in the batch was reached
        /// </summary>
        Time = 0,

        /// <summary>
        /// The maximum number of messages in the batch was reached
        /// </summary>
        Size = 1,

        /// <summary>
        /// A batch was forced, likely due to a previously faulted message being retried
        /// </summary>
        Forced = 2
    }
}
