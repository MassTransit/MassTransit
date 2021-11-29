namespace MassTransit.Middleware.CircuitBreaker
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Represents a closed, normally operating circuit breaker state
    /// </summary>
    public class ClosedBehavior :
        ICircuitBreakerBehavior
    {
        readonly ICircuitBreaker _breaker;
        readonly Timer _timer;
        int _attemptCount;
        int _failureCount;
        int _successCount;

        public ClosedBehavior(ICircuitBreaker breaker)
        {
            _breaker = breaker;
            _timer = new Timer(Reset, null, breaker.OpenDuration, breaker.OpenDuration);
        }

        bool IsActive => _attemptCount > _breaker.ActiveThreshold;

        Task ICircuitBreakerBehavior.PreSend()
        {
            Interlocked.Increment(ref _attemptCount);

            return Task.CompletedTask;
        }

        Task ICircuitBreakerBehavior.PostSend()
        {
            Interlocked.Increment(ref _successCount);

            return Task.CompletedTask;
        }

        async Task ICircuitBreakerBehavior.SendFault(Exception exception)
        {
            var failureCount = Interlocked.Increment(ref _failureCount);

            if (IsActive && TripThresholdExceeded(failureCount))
            {
                await _breaker.Open(exception, this).ConfigureAwait(false);
                _timer.Dispose();
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                State = "closed",
                AttemptCount = _attemptCount,
                SuccessCount = _successCount,
                FailureCount = _failureCount
            });
        }

        bool TripThresholdExceeded(int failureCount)
        {
            return failureCount * 100L / _attemptCount >= _breaker.TripThreshold;
        }

        void Reset(object state)
        {
            lock (_breaker)
            {
                _attemptCount = 0;
                _successCount = 0;
                _failureCount = 0;
            }
        }
    }
}
