namespace MassTransit.Contracts
{
    /// <summary>
    /// Set the rate limit of the RateLimitFilter
    /// </summary>
    public interface SetRateLimit
    {
        /// <summary>
        /// The new rate limit for the filter
        /// </summary>
        int RateLimit { get; }
    }
}
