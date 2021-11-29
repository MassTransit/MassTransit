namespace MassTransit.JobService
{
    using System;
    using System.Threading.Tasks;


    public class JobServiceBusObserver :
        IBusObserver
    {
        readonly IJobService _jobService;

        public JobServiceBusObserver(IJobService jobService)
        {
            _jobService = jobService;
        }

        public void PostCreate(IBus bus)
        {
        }

        public void CreateFaulted(Exception exception)
        {
        }

        public Task PreStart(IBus bus)
        {
            return Task.CompletedTask;
        }

        public async Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            LogContext.Debug?.Log("Job Service starting: {InstanceAddress}", _jobService.InstanceAddress);

            await busReady.ConfigureAwait(false);

            await _jobService.BusStarted(bus).ConfigureAwait(false);

            LogContext.Info?.Log("Job Service started: {InstanceAddress}", _jobService.InstanceAddress);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public async Task PreStop(IBus bus)
        {
            LogContext.Debug?.Log("Job Service shutting down: {InstanceAddress}", _jobService.InstanceAddress);

            await _jobService.Stop(bus).ConfigureAwait(false);

            LogContext.Info?.Log("Job Service shut down: {InstanceAddress}", _jobService.InstanceAddress);
        }

        public Task PostStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
