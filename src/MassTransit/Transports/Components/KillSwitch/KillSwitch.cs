namespace MassTransit.Transports.Components
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;


    /// <summary>
    /// The KillSwitch monitors a receive endpoint, stopping and restarting as necessary in the presence of exceptions.
    /// </summary>
    public class KillSwitch :
        IKillSwitch,
        IConsumeObserver
    {
        readonly ILogContext _logContext;
        readonly KillSwitchOptions _options;
        readonly IReceiveEndpoint _receiveEndpoint;
        IKillSwitchState _state;

        public KillSwitch(KillSwitchOptions options, IReceiveEndpoint receiveEndpoint)
        {
            _options = options;
            _receiveEndpoint = receiveEndpoint;

            _logContext = LogContext.Current;

            _state = new StartedKillSwitchState(this);
        }

        public Task PreConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _state.PreConsume(context);
        }

        public Task PostConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _state.PostConsume(context);
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _state.ConsumeFault(context, exception);
        }

        ILogContext IKillSwitch.LogContext => _logContext;

        public int ActivationThreshold => _options.ActivationThreshold;
        public int TripThreshold => _options.TripThreshold;
        public TimeSpan TrackingPeriod => _options.TrackingPeriod;
        public TimeSpan RestartTimeout => _options.RestartTimeout;
        public IExceptionFilter ExceptionFilter => _options.ExceptionFilter;

        public void Started(IKillSwitchState previousState)
        {
            var started = new StartedKillSwitchState(this);

            Interlocked.CompareExchange(ref _state, started, previousState);
            if (_state != started)
                return;

            started.Activate();
        }

        public void Restart(Exception exception, IKillSwitchState previousState)
        {
            var restarting = new RestartingKillSwitchState(this, exception);

            Interlocked.CompareExchange(ref _state, restarting, previousState);
            if (_state != restarting)
                return;

            Task.Run(() => RestartReceiveEndpoint());
        }

        public void Stop(Exception exception, IKillSwitchState previousState)
        {
            var stopped = new StoppedKillSwitchState(this, exception);

            Interlocked.CompareExchange(ref _state, stopped, previousState);
            if (_state != stopped)
                return;

            if (previousState is StartedKillSwitchState started)
                started.LogThreshold();

            Task.Run(() => StopReceiveEndpoint(stopped));
        }

        async Task StopReceiveEndpoint(StoppedKillSwitchState state)
        {
            try
            {
                await _receiveEndpoint.Stop().ConfigureAwait(false);

                var startTime = DateTime.UtcNow + RestartTimeout;

                LogContext.Info?.Log("Kill Switch stopped endpoint, restarting at {StartTime}: {InputAddress}", startTime, _receiveEndpoint.InputAddress);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Kill Switch failed to stop endpoint: {InputAddress}", _receiveEndpoint.InputAddress);
            }
            finally
            {
                state.Activate();
            }
        }

        async Task RestartReceiveEndpoint()
        {
            try
            {
                var handle = _receiveEndpoint.Start();

                await handle.Ready.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Kill Switch failed to restart endpoint: {InputAddress}", _receiveEndpoint.InputAddress);
            }
        }
    }
}
