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
        IReceiveEndpointObserver,
        IConsumeObserver,
        IActivityObserver
    {
        readonly ILogContext _logContext;
        readonly KillSwitchOptions _options;
        ConnectHandle _consumeConnectHandle;
        IReceiveEndpoint _receiveEndpoint;
        IKillSwitchState _state;

        public KillSwitch(KillSwitchOptions options)
        {
            _options = options;

            _logContext = LogContext.Current;
        }

        public Task PreExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return _state.PreConsume(context);
        }

        public Task PostExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return _state.PostConsume(context);
        }

        public Task ExecuteFault<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return _state.ConsumeFault(context, exception);
        }

        public Task PreCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return _state.PreConsume(context);
        }

        public Task PostCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return _state.PostConsume(context);
        }

        public Task CompensateFail<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return _state.ConsumeFault(context, exception);
        }

        public Task PreConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            return _state.PreConsume(context);
        }

        public Task PostConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            return _state.PostConsume(context);
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
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

        public Task Ready(ReceiveEndpointReady ready)
        {
            _receiveEndpoint = ready.ReceiveEndpoint;

            _consumeConnectHandle ??= _receiveEndpoint.ConnectConsumeObserver(this);
            Started(null);
            return Task.CompletedTask;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return Task.CompletedTask;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            return Task.CompletedTask;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            return Task.CompletedTask;
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
