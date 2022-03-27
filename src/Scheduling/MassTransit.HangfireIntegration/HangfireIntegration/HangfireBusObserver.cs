namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Hangfire;
    using Microsoft.Extensions.Options;


    /// <summary>
    /// Used to start and stop an in-memory scheduler using Hangfire
    /// </summary>
    class HangfireBusObserver :
        IBusObserver
    {
        readonly JobStorage _jobStorage;
        readonly IOptions<BackgroundJobServerOptions> _options;
        BackgroundJobServer? _server;

        public HangfireBusObserver(JobStorage jobStorage, IOptions<BackgroundJobServerOptions> options)
        {
            _jobStorage = jobStorage;
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
            var options = _options.Value;
            options.Queues = new[] { HangfireEndpointOptions.DefaultQueueName };
            options.ServerName = $"MT-Server-{NewId.NextGuid():N}";

            _server = new BackgroundJobServer(options, _jobStorage);

            await busReady.ConfigureAwait(false);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task PreStop(IBus bus)
        {
            _server?.SendStop();

            return Task.CompletedTask;
        }

        public async Task PostStop(IBus bus)
        {
            if (_server != null)
            {
                using var cancellationTokenSource = new CancellationTokenSource(_options.Value.ShutdownTimeout);
                await _server.WaitForShutdownAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                _server.Dispose();
            }
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
