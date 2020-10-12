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
        readonly IScheduler _scheduler;
        readonly Uri _schedulerEndpointAddress;
        readonly IJobFactory _defaultJobFactory;

        /// <summary>
        /// Creates the bus observer to initialize the Quartz scheduler.
        /// </summary>
        /// <param name="scheduler">The Quartz scheduler instance</param>
        /// <param name="schedulerEndpointAddress">The endpoint address of the quartz service</param>
        /// <param name="defaultJobFactory">Optional, can be used to specify a job factory for non-MassTransit job types</param>
        public SchedulerBusObserver(IScheduler scheduler, Uri schedulerEndpointAddress, IJobFactory defaultJobFactory = default)
        {
            _scheduler = scheduler;
            _schedulerEndpointAddress = schedulerEndpointAddress;
            _defaultJobFactory = defaultJobFactory;
        }

        public Task PostCreate(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task CreateFaulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStart(IBus bus)
        {
            return TaskUtil.Completed;
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
