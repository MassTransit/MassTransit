namespace MassTransit.QuartzIntegration.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Quartz;
    using Quartz.Spi;
    using Util;


    /// <summary>
    /// Used to start and stop an in-memory scheduler using Quartz
    /// </summary>
    public class SchedulerBusObserver :
        IBusObserver
    {
        readonly IJobFactory _defaultJobFactory;
        readonly Uri _schedulerEndpointAddress;
        readonly ISchedulerFactory _schedulerFactory;
        readonly TaskCompletionSource<IScheduler> _schedulerSource;
        IScheduler _scheduler;

        /// <summary>
        /// Creates the bus observer to initialize the Quartz scheduler.
        /// </summary>
        /// <param name="schedulerFactory">Used to create the scheduler at bus start</param>
        /// <param name="schedulerEndpointAddress">The endpoint address of the quartz service</param>
        /// <param name="defaultJobFactory">Optional, can be used to specify a job factory for non-MassTransit job types</param>
        public SchedulerBusObserver(ISchedulerFactory schedulerFactory, Uri schedulerEndpointAddress, IJobFactory defaultJobFactory = default)
        {
            _schedulerFactory = schedulerFactory;
            _schedulerEndpointAddress = schedulerEndpointAddress;
            _defaultJobFactory = defaultJobFactory;

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
                _scheduler = await _schedulerFactory.GetScheduler().ConfigureAwait(false);

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
            LogContext.Debug?.Log("Quartz Scheduler Starting: {InputAddress} ({Name}/{InstanceId})", _schedulerEndpointAddress, _scheduler.SchedulerName,
                _scheduler.SchedulerInstanceId);

            await busReady.ConfigureAwait(false);

            _scheduler.JobFactory = new MassTransitJobFactory(bus, _defaultJobFactory);

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
