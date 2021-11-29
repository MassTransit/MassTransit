namespace MassTransit.Middleware.CircuitBreaker
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Represents a circuit that is unavailable, with a timer waiting to partially close
    /// the circuit.
    /// </summary>
    public class OpenBehavior :
        ICircuitBreakerBehavior
    {
        readonly ICircuitBreaker _breaker;
        readonly Stopwatch _elapsed;
        readonly Exception _exception;
        readonly IEnumerator<TimeSpan> _timeoutEnumerator;
        readonly Timer _timer;

        public OpenBehavior(ICircuitBreaker breaker, Exception exception, IEnumerator<TimeSpan> timeoutEnumerator)
        {
            _breaker = breaker;
            _exception = exception;
            _timeoutEnumerator = timeoutEnumerator;

            _timer = GetTimer(timeoutEnumerator);
            _elapsed = Stopwatch.StartNew();
        }

        Task ICircuitBreakerBehavior.PreSend()
        {
            throw _exception;
        }

        Task ICircuitBreakerBehavior.PostSend()
        {
            return Task.CompletedTask;
        }

        Task ICircuitBreakerBehavior.SendFault(Exception exception)
        {
            return Task.CompletedTask;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var timeout = _timeoutEnumerator.Current;
            context.Set(new
            {
                State = "open",
                Exception = _exception,
                Timeout = timeout,
                Remaining = timeout - _elapsed.Elapsed
            });
        }

        Timer GetTimer(IEnumerator<TimeSpan> timeoutEnumerator)
        {
            timeoutEnumerator.MoveNext();

            return new Timer(PartiallyCloseCircuit, this, timeoutEnumerator.Current, TimeSpan.FromMilliseconds(-1));
        }

        void PartiallyCloseCircuit(object state)
        {
            _breaker.ClosePartially(_exception, _timeoutEnumerator, this);
            _timer.Dispose();
        }
    }
}
