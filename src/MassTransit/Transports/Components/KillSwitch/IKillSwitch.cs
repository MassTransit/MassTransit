namespace MassTransit.Transports.Components
{
    using System;
    using Logging;


    public interface IKillSwitch
    {
        ILogContext LogContext { get; }

        /// <summary>
        /// The minimum number of attempts before the breaker can possibly trip
        /// </summary>
        int ActivationThreshold { get; }

        /// <summary>
        /// The number of failures before opening the circuit breaker
        /// </summary>
        int TripThreshold { get; }

        /// <summary>
        /// Window duration before attempt/success/failure counts are reset
        /// </summary>
        TimeSpan TrackingPeriod { get; }

        /// <summary>
        /// The wait time before restarting
        /// </summary>
        TimeSpan RestartTimeout { get; }

        /// <summary>
        /// Matches the supported exceptions for the kill switch
        /// </summary>
        IExceptionFilter ExceptionFilter { get; }

        /// <summary>
        /// Stop for the restart timeout
        /// </summary>
        /// <param name="exception">The exception to return when the circuit breaker is accessed</param>
        /// <param name="previousState"></param>
        void Stop(Exception exception, IKillSwitchState previousState);

        /// <summary>
        /// Restart, monitoring exception rates until they stabilize
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="previousState"></param>
        void Restart(Exception exception, IKillSwitchState previousState);

        /// <summary>
        /// Transition to the Started state, where exception rates are below the trip threshold
        /// </summary>
        /// <param name="previousState"></param>
        void Started(IKillSwitchState previousState);
    }
}
