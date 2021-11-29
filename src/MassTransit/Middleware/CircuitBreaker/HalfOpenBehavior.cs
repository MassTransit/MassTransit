namespace MassTransit.Middleware.CircuitBreaker
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Executes until the success count is met. If a fault occurs before the success
    /// count is reached, the circuit reopens.
    /// </summary>
    public class HalfOpenBehavior :
        ICircuitBreakerBehavior
    {
        readonly ICircuitBreaker _breaker;
        readonly Exception _exception;
        readonly IEnumerator<TimeSpan> _timeoutEnumerator;
        int _attemptCount;

        public HalfOpenBehavior(ICircuitBreaker breaker, Exception exception, IEnumerator<TimeSpan> timeoutEnumerator)
        {
            _breaker = breaker;
            _exception = exception;
            _timeoutEnumerator = timeoutEnumerator;
        }

        bool IsActive => _attemptCount > _breaker.ActiveThreshold;

        Task ICircuitBreakerBehavior.PreSend()
        {
            Interlocked.Increment(ref _attemptCount);

            return Task.CompletedTask;
        }

        async Task ICircuitBreakerBehavior.PostSend()
        {
            if (IsActive)
            {
                await _breaker.Close(this).ConfigureAwait(false);
                _timeoutEnumerator.Dispose();
            }
        }

        Task ICircuitBreakerBehavior.SendFault(Exception exception)
        {
            return _breaker.Open(_exception, this, _timeoutEnumerator);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                State = "halfOpen",
                AttemptCount = _attemptCount
            });
        }
    }
}
