namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CircuitBreaker;


    public class CircuitBreakerFilter<TContext> :
        IFilter<TContext>,
        ICircuitBreaker
        where TContext : class, PipeContext
    {
        readonly IExceptionFilter _exceptionFilter;
        readonly CircuitBreakerSettings _settings;
        ICircuitBreakerBehavior _behavior;

        public CircuitBreakerFilter(CircuitBreakerSettings settings, IExceptionFilter exceptionFilter)
        {
            _settings = settings;
            _exceptionFilter = exceptionFilter;

            _behavior = new ClosedBehavior(this);
        }

        public TimeSpan OpenDuration => _settings.TrackingPeriod;

        Task ICircuitBreaker.Open(Exception exception, ICircuitBreakerBehavior behavior, IEnumerator<TimeSpan> timeoutEnumerator)
        {
            if (timeoutEnumerator == null)
                timeoutEnumerator = _settings.ResetTimeout.GetEnumerator();

            var openBehavior = new OpenBehavior(this, exception, timeoutEnumerator);

            Interlocked.CompareExchange(ref _behavior, openBehavior, behavior);
            if (_behavior == openBehavior)
                return _settings.Router?.PublishCircuitBreakerOpened(exception) ?? Task.CompletedTask;

            return Task.CompletedTask;
        }

        Task ICircuitBreaker.Close(ICircuitBreakerBehavior behavior)
        {
            var closedBehavior = new ClosedBehavior(this);
            Interlocked.CompareExchange(ref _behavior, closedBehavior, behavior);
            if (_behavior == closedBehavior)
                return _settings.Router?.PublishCircuitBreakerClosed() ?? Task.CompletedTask;

            return Task.CompletedTask;
        }

        Task ICircuitBreaker.ClosePartially(Exception exception, IEnumerator<TimeSpan> timeoutEnumerator, ICircuitBreakerBehavior behavior)
        {
            Interlocked.CompareExchange(ref _behavior, new HalfOpenBehavior(this, exception, timeoutEnumerator), behavior);

            return Task.CompletedTask;
        }

        public int TripThreshold => _settings.TripThreshold;

        public int ActiveThreshold => _settings.ActiveThreshold;

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            try
            {
                await _behavior.PreSend().ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);

                await _behavior.PostSend().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!_exceptionFilter.Match(ex))
                    throw;

                await _behavior.SendFault(ex).ConfigureAwait(false);

                throw;
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("circuitBreaker");
            scope.Set(new
            {
                ActiveCount = _settings.ActiveThreshold,
                _settings.TripThreshold,
                Duration = _settings.TrackingPeriod,
                ResetTimeout = _settings.ResetTimeout.Take(10).ToArray()
            });

            _behavior.Probe(scope);
        }
    }
}
