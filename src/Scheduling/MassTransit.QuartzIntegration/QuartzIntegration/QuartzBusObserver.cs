namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Quartz;


    /// <summary>
    /// Used by container-based Quartz configurations, to start/stop Quartz along with the bus.
    /// </summary>
    public class QuartzBusObserver :
        IBusObserver
    {
        readonly IOptions<QuartzHostedServiceOptions> _options;
        readonly ISchedulerFactory _schedulerFactory;
        IScheduler? _scheduler;

        public QuartzBusObserver(ISchedulerFactory schedulerFactory, IOptions<QuartzHostedServiceOptions> options)
        {
            _schedulerFactory = schedulerFactory;
            _options = options;
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
            await busReady.ConfigureAwait(false);

            _scheduler = await _schedulerFactory.GetScheduler().ConfigureAwait(false);

            if (_options.Value.StartDelay.HasValue)
                await _scheduler.StartDelayed(_options.Value.StartDelay.Value).ConfigureAwait(false);
            else
                await _scheduler.Start().ConfigureAwait(false);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task PreStop(IBus bus)
        {
            return _scheduler != null ? _scheduler.Standby() : Task.CompletedTask;
        }

        public Task PostStop(IBus bus)
        {
            return _scheduler != null ? _scheduler.Shutdown(_options.Value.WaitForJobsToComplete) : Task.CompletedTask;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
