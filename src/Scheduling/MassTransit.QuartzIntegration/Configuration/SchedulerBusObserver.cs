namespace MassTransit.QuartzIntegration.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Quartz;
    using Util;

    /// <summary>
    /// Used to start and stop an in-memory scheduler using Quartz
    /// </summary>
    public class SchedulerBusObserver :
        IBusObserver
    {
        readonly InMemorySchedulerOptions _options;
        readonly Uri _schedulerEndpointAddress;
        readonly TaskCompletionSource<IScheduler> _schedulerSource;
        IScheduler _scheduler;

        /// <summary>
        /// Creates the bus observer to initialize the Quartz scheduler.
        /// </summary>
        /// <param name="options">Configuration to initialize with.</param>
        public SchedulerBusObserver(InMemorySchedulerOptions options)
        {
            _options = options;
            _schedulerEndpointAddress = new Uri($"queue:{options.QueueName}");
            _schedulerSource = TaskUtil.GetTask<IScheduler>();
        }

        public Task<IScheduler> Scheduler => _schedulerSource.Task;

        public Task PostCreate(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task CreateFaulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public async Task PreStart(IBus bus)
        {
            LogContext.Debug?.Log("Creating Quartz Scheduler: {InputAddress}", _schedulerEndpointAddress);

            try
            {
                _scheduler = await _options.SchedulerFactory.GetScheduler().ConfigureAwait(false);

                _schedulerSource.TrySetResult(_scheduler);
            }
            catch (Exception exception)
            {
                _schedulerSource.TrySetException(exception);
                throw;
            }
        }

        public async Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            if (!_options.StartScheduler)
            {
                LogContext.Debug?.Log("Quartz Scheduler: {InputAddress} ({Name}/{InstanceId}) initialized, but not started",
                    _schedulerEndpointAddress,
                    _scheduler.SchedulerName,
                    _scheduler.SchedulerInstanceId);
            }

            LogContext.Debug?.Log("Quartz Scheduler Starting: {InputAddress} ({Name}/{InstanceId})", _schedulerEndpointAddress, _scheduler.SchedulerName,
                _scheduler.SchedulerInstanceId);

            await busReady.ConfigureAwait(false);

            _scheduler.JobFactory = new MassTransitJobFactory(bus, _options.JobFactory);

            await _scheduler.Start().ConfigureAwait(false);

            LogContext.Debug?.Log("Quartz Scheduler Started: {InputAddress} ({Name}/{InstanceId})", _schedulerEndpointAddress, _scheduler.SchedulerName,
                _scheduler.SchedulerInstanceId);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public async Task PreStop(IBus bus)
        {
            if (!_options.StartScheduler)
            {
                return;;
            }

            await _scheduler.Standby().ConfigureAwait(false);

            LogContext.Debug?.Log("Quartz Scheduler Paused: {InputAddress} ({Name}/{InstanceId})", _schedulerEndpointAddress, _scheduler.SchedulerName,
                _scheduler.SchedulerInstanceId);
        }

        public async Task PostStop(IBus bus)
        {
            await _scheduler.Shutdown().ConfigureAwait(false);

            LogContext.Debug?.Log("Quartz Scheduler Stopped: {InputAddress} ({Name}/{InstanceId})", _schedulerEndpointAddress, _scheduler.SchedulerName,
                _scheduler.SchedulerInstanceId);
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}
