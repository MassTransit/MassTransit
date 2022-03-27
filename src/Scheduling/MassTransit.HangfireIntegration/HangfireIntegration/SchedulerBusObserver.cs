namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Hangfire;


    /// <summary>
    /// Used to start and stop an in-memory scheduler using Hangfire
    /// </summary>
    class SchedulerBusObserver :
        IBusObserver
    {
        readonly HangfireSchedulerOptions _options;
        readonly Uri _schedulerEndpointAddress;
        BackgroundJobServer? _server;

        public SchedulerBusObserver(HangfireSchedulerOptions options)
        {
            _schedulerEndpointAddress = new Uri($"queue:{options.QueueName}");
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
            var backgroundJobServerOptions = new BackgroundJobServerOptions
            {
                TimeZoneResolver = _options.ComponentResolver.TimeZoneResolver,
                FilterProvider = _options.ComponentResolver.JobFilterProvider,
                ServerName = $"MT-Server-{NewId.NextGuid():N}"
            };

            _options.ConfigureServer?.Invoke(backgroundJobServerOptions);

            backgroundJobServerOptions.Activator = new MassTransitJobActivator(bus);
            backgroundJobServerOptions.Queues = new[] { HangfireEndpointOptions.DefaultQueueName };

            _server = new BackgroundJobServer(
                backgroundJobServerOptions,
                _options.ComponentResolver.JobStorage,
                _options.ComponentResolver.BackgroundProcesses);

            LogContext.Debug?.Log("Hangfire Scheduler Starting: {InputAddress}", _schedulerEndpointAddress);

            await busReady.ConfigureAwait(false);

            LogContext.Debug?.Log("Hangfire Scheduler Started: {InputAddress}", _schedulerEndpointAddress);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task PreStop(IBus bus)
        {
            _server?.SendStop();

            LogContext.Debug?.Log("Hangfire Scheduler Paused: {InputAddress}", _schedulerEndpointAddress);

            return Task.CompletedTask;
        }

        public async Task PostStop(IBus bus)
        {
            if (_server != null)
            {
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await _server.WaitForShutdownAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                _server.Dispose();
            }

            LogContext.Debug?.Log("Hangfire Scheduler Stopped: {InputAddress}", _schedulerEndpointAddress);
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
