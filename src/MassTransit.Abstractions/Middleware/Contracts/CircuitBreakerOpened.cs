namespace MassTransit.Contracts
{
    using System;


    public interface CircuitBreakerOpened
    {
        /// <summary>
        /// The exception that caused the circuit breaker to open
        /// </summary>
        Exception Exception { get; }
    }
}
