namespace MassTransit.Middleware.CircuitBreaker
{
    using System;
    using System.Collections.Generic;


    public interface CircuitBreakerSettings
    {
        /// <summary>
        /// The window duration to keep track of errors before they fall off the breaker state
        /// </summary>
        TimeSpan TrackingPeriod { get; }

        /// <summary>
        /// The time to wait after the breaker has opened before attempting to close it
        /// </summary>
        IEnumerable<TimeSpan> ResetTimeout { get; }

        /// <summary>
        /// A percentage of how many failures versus successful calls before the breaker
        /// is opened. Should be 0-100, but seriously like 5-10.
        /// </summary>
        int TripThreshold { get; }

        /// <summary>
        /// The active count of attempts before the circuit breaker can be tripped
        /// </summary>
        int ActiveThreshold { get; }

        /// <summary>
        /// The router used to publish events related to the circuit breaker behavior
        /// </summary>
        IPipeRouter Router { get; }
    }
}
