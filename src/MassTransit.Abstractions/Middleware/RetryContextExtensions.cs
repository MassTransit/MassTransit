namespace MassTransit
{
    public static class RetryContextExtensions
    {
        /// <summary>
        /// If within a retry attempt, the return value is greater than zero and indicates the number of the retry attempt
        /// in progress.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The retry attempt number, 0 = first time, >= 1 = retry</returns>
        public static int GetRetryAttempt(this ConsumeContext context)
        {
            return context.TryGetPayload(out ConsumeRetryContext? retryContext) ? retryContext!.RetryAttempt : 0;
        }

        /// <summary>
        /// If within a retry attempt, the return value indicates the number of retry attempts that have already occurred.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The number of retries that have already been attempted, 0 = first time or first retry, >= 1 = subsequent retry</returns>
        public static int GetRetryCount(this ConsumeContext context)
        {
            return context.TryGetPayload(out ConsumeRetryContext? retryContext) ? retryContext!.RetryCount : 0;
        }

        /// <summary>
        /// If the message is being redelivered, returns the redelivery attempt
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The retry attempt number, 0 = first time, >= 1 = retry</returns>
        public static int GetRedeliveryCount(this ConsumeContext context)
        {
            return context.Headers.Get(MessageHeaders.RedeliveryCount, default(int?)) ?? 0;
        }
    }
}
