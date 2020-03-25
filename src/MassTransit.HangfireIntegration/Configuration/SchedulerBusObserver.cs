namespace MassTransit.HangfireIntegration.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Hangfire;
    using Util;


    /// <summary>
    /// Used to start and stop an in-memory scheduler using Quartz
    /// </summary>
    class SchedulerBusObserver :
        IBusObserver
    {
        readonly IHangfireComponentResolver _hangfireComponentResolver;
        readonly Uri _schedulerEndpointAddress;
        readonly Action<BackgroundJobServerOptions> _configureServer;
        BackgroundJobServer _server;

        public SchedulerBusObserver(IHangfireComponentResolver hangfireComponentResolver, Uri schedulerEndpointAddress,
            Action<BackgroundJobServerOptions> configureServer)
        {
            _hangfireComponentResolver = hangfireComponentResolver;
            _schedulerEndpointAddress = schedulerEndpointAddress;
            _configureServer = configureServer;
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
            var backgroundJobServerOptions = new BackgroundJobServerOptions
            {
                Activator = new MassTransitJobActivator(bus),
                TimeZoneResolver = _hangfireComponentResolver.TimeZoneResolver,
                FilterProvider = _hangfireComponentResolver.JobFilterProvider
            };
            _configureServer?.Invoke(backgroundJobServerOptions);

            _server = new BackgroundJobServer(
                backgroundJobServerOptions,
                _hangfireComponentResolver.JobStorage,
                _hangfireComponentResolver.BackgroundProcesses);
            LogContext.Debug?.Log("Quartz Scheduler Starting: {InputAddress}", _schedulerEndpointAddress);

            await busReady.ConfigureAwait(false);

            LogContext.Debug?.Log("Quartz Scheduler Started: {InputAddress}", _schedulerEndpointAddress);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStop(IBus bus)
        {
            _server.SendStop();

            LogContext.Debug?.Log("Quartz Scheduler Paused: {InputAddress}", _schedulerEndpointAddress);

            return TaskUtil.Completed;
        }

        public Task PostStop(IBus bus)
        {
            _server.Dispose();

            LogContext.Debug?.Log("Quartz Scheduler Stopped: {InputAddress}", _schedulerEndpointAddress);

            return TaskUtil.Completed;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}
