namespace MassTransit.Turnout.Components
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Util;


    public class JobServiceBusObserver :
        IBusObserver
    {
        readonly IJobService _jobService;

        public JobServiceBusObserver(IJobService jobService)
        {
            _jobService = jobService;
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
            LogContext.Debug?.Log("Job Service starting: {InstanceAddress}", _jobService.InstanceAddress);

            await busReady.ConfigureAwait(false);

            LogContext.Info?.Log("Job Service started: {InstanceAddress}", _jobService.InstanceAddress);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public async Task PreStop(IBus bus)
        {
            LogContext.Debug?.Log("Job Service shutting down: {InstanceAddress}", _jobService.InstanceAddress);

            await _jobService.Stop().ConfigureAwait(false);

            LogContext.Info?.Log("Job Service shut down: {InstanceAddress}", _jobService.InstanceAddress);
        }

        public Task PostStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}
