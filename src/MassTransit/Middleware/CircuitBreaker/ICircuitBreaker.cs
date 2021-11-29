namespace MassTransit.Middleware.CircuitBreaker
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    /// <summary>
    /// Provides access to a circuit breaker from a state object
    /// </summary>
    public interface ICircuitBreaker
    {
        /// <summary>
        /// The number of failures before opening the circuit breaker
        /// </summary>
        int TripThreshold { get; }

        /// <summary>
        /// The minimum number of attempts before the breaker can possibly trip
        /// </summary>
        int ActiveThreshold { get; }

        /// <summary>
        /// Window duration before attempt/success/failure counts are reset
        /// </summary>
        TimeSpan OpenDuration { get; }

        /// <summary>
        /// Open the circuit breaker, preventing any further access to the resource until
        /// the timer expires
        /// </summary>
        /// <param name="exception">The exception to return when the circuit breaker is accessed</param>
        /// <param name="behavior"></param>
        /// <param name="timeoutEnumerator">A previously created enumerator for a timeout period</param>
        Task Open(Exception exception, ICircuitBreakerBehavior behavior, IEnumerator<TimeSpan> timeoutEnumerator = null);

        /// <summary>
        /// Partially open the circuit breaker, allowing the eventual return to a closed
        /// state
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="timeoutEnumerator"></param>
        /// <param name="behavior"></param>
        Task ClosePartially(Exception exception, IEnumerator<TimeSpan> timeoutEnumerator, ICircuitBreakerBehavior behavior);

        /// <summary>
        /// Close the circuit breaker, allowing normal execution
        /// </summary>
        /// <param name="behavior"></param>
        Task Close(ICircuitBreakerBehavior behavior);
    }
}
