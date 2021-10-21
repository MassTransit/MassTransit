namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Util;


    public class BusHostedService :
        IHostedService, IDisposable
    {
        readonly IBusControl _bus;
        readonly bool _waitUntilStarted;
        Task<BusHandle> _startTask;
        bool _stopped = false;

        public BusHostedService(IBusControl bus, bool waitUntilStarted)
        {
            _bus = bus;
            _waitUntilStarted = waitUntilStarted;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask = _bus.StartAsync(cancellationToken);

            return _startTask.IsCompleted || _waitUntilStarted
                ? _startTask
                : TaskUtil.Completed;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_stopped)
            {
                await _bus.StopAsync(cancellationToken);
                _stopped = true;
            }
        }

        public void Dispose()
        {
            if (!_stopped)
            {
                _bus.Stop();
                _stopped = true;
            }
        }
    }
}
