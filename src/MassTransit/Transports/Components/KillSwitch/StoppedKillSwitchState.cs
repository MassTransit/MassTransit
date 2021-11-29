namespace MassTransit.Transports.Components
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;


    public class StoppedKillSwitchState :
        IKillSwitchState
    {
        Stopwatch _elapsed;
        readonly Exception _exception;
        readonly IKillSwitch _killSwitch;
        Timer _timer;

        public StoppedKillSwitchState(IKillSwitch killSwitch, Exception exception)
        {
            _killSwitch = killSwitch;
            _exception = exception;
        }

        public void Probe(ProbeContext context)
        {
            context.Set(new
            {
                State = "stopped",
                Exception = _exception,
                Timeout = _killSwitch.RestartTimeout,
                Remaining = _killSwitch.RestartTimeout - _elapsed?.Elapsed
            });
        }

        public void Activate()
        {
            _elapsed = Stopwatch.StartNew();
            _timer = new Timer(Restart, this, _killSwitch.RestartTimeout, TimeSpan.FromMilliseconds(-1));
        }

        public Task PreConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            return Task.CompletedTask;
        }

        void Restart(object state)
        {
            LogContext.SetCurrentIfNull(_killSwitch.LogContext);

            try
            {
                _killSwitch.Restart(_exception, this);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Failed to restart receive endpoint");
            }
            finally
            {
                _timer?.Dispose();
            }
        }
    }
}
