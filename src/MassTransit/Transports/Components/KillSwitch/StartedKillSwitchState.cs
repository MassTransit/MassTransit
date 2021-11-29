namespace MassTransit.Transports.Components
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class StartedKillSwitchState :
        IKillSwitchState
    {
        readonly IKillSwitch _killSwitch;
        int _attemptCount;
        int _failureCount;
        int _successCount;
        Timer _timer;

        public StartedKillSwitchState(IKillSwitch killSwitch)
        {
            _killSwitch = killSwitch;
        }

        bool IsActive => _attemptCount > _killSwitch.ActivationThreshold;

        public void Probe(ProbeContext context)
        {
            context.Set(new
            {
                State = "started",
                AttemptCount = _attemptCount,
                SuccessCount = _successCount,
                FailureCount = _failureCount
            });
        }

        public void LogThreshold()
        {
            LogContext.Debug?.Log("Kill Switch threshold reached, failures: {FailureCount}, attempts: {AttemptCount}", _failureCount, _attemptCount);
        }

        public Task PreConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            Interlocked.Increment(ref _attemptCount);

            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            Interlocked.Increment(ref _successCount);

            return Task.CompletedTask;
        }

        public async Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            if (_killSwitch.ExceptionFilter.Match(exception))
            {
                var failureCount = Interlocked.Increment(ref _failureCount);

                if (IsActive && TripThresholdExceeded(failureCount))
                {
                    _killSwitch.Stop(exception, this);
                    _timer?.Dispose();
                }
            }
        }

        public void Activate()
        {
            _timer = new Timer(Reset, null, _killSwitch.TrackingPeriod, _killSwitch.TrackingPeriod);
        }

        bool TripThresholdExceeded(int failureCount)
        {
            return failureCount * 100L / _attemptCount >= _killSwitch.TripThreshold;
        }

        void Reset(object state)
        {
            lock (_killSwitch)
            {
                _attemptCount = 0;
                _successCount = 0;
                _failureCount = 0;
            }
        }
    }
}
