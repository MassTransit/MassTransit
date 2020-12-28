namespace MassTransit.Transports.Components
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    class RestartingKillSwitchState :
        IKillSwitchState
    {
        readonly Exception _exception;
        readonly IKillSwitch _killSwitch;
        int _attemptCount;

        public RestartingKillSwitchState(IKillSwitch killSwitch, Exception exception)
        {
            _killSwitch = killSwitch;
            _exception = exception;
        }

        bool IsActive => _attemptCount > _killSwitch.ActivationThreshold;

        public void Probe(ProbeContext context)
        {
            context.Set(new
            {
                State = "restarting",
                AttemptCount = _attemptCount
            });
        }

        public Task PreConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            Interlocked.Increment(ref _attemptCount);

            return TaskUtil.Completed;
        }

        public Task PostConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            if (IsActive)
                _killSwitch.Started(this);

            return TaskUtil.Completed;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            if (_killSwitch.ExceptionFilter.Match(exception))
                _killSwitch.Stop(_exception, this);

            return TaskUtil.Completed;
        }
    }
}
