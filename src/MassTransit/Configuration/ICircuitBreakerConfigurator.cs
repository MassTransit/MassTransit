namespace MassTransit
{
    using System;
    using Middleware;


    /// <summary>
    /// Configure the settings on the circuit breaker
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface ICircuitBreakerConfigurator<TContext> :
        IExceptionConfigurator
        where TContext : class, PipeContext
    {
        /// <summary>
        /// The period after which the attempt/failure counts are reset.
        /// </summary>
        TimeSpan TrackingPeriod { set; }

        /// <summary>
        /// The percentage of attempts that must fail before the circuit breaker trips into
        /// an open state.
        /// </summary>
        int TripThreshold { set; }

        /// <summary>
        /// The number of attempts that must occur before the circuit breaker becomes active. Until the
        /// breaker activates, it will not open on failure
        /// </summary>
        int ActiveThreshold { set; }

        /// <summary>
        /// Sets a specific reset interval for the circuit to attempt to close after being tripped.
        /// By default, this is an incrementing scale up to one minute.
        /// </summary>
        /// <value></value>
        TimeSpan ResetInterval { set; }

        /// <summary>
        /// Configure a router for sending events from the circuit breaker
        /// </summary>
        IPipeRouter Router { set; }
    }
}
