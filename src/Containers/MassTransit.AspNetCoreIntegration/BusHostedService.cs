namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Util;


    public class BusHostedService :
        IHostedService,
        IDisposable
    {
        readonly IBusControl _bus;
        readonly TimeSpan? _startTimeout;
        readonly TimeSpan? _stopTimeout;
        readonly bool _waitUntilStarted;
        Task<BusHandle> _startTask;
        bool _stopped;

        public BusHostedService(IBusControl bus, bool waitUntilStarted, TimeSpan? startTimeout = null, TimeSpan? stopTimeout = null)
        {
            _bus = bus;
            _waitUntilStarted = waitUntilStarted;
            _startTimeout = startTimeout;
            _stopTimeout = stopTimeout;
        }

        public void Dispose()
        {
            if (_stopped)
                return;

            if (_stopTimeout.HasValue)
                _bus.Stop(_stopTimeout.Value);
            else
                _bus.Stop();

            _stopped = true;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask = _startTimeout.HasValue
                ? _bus.StartAsync(_startTimeout.Value)
                : _bus.StartAsync(cancellationToken);

            return _startTask.IsCompleted || _waitUntilStarted
                ? _startTask
                : TaskUtil.Completed;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_stopped)
            {
                await (_stopTimeout.HasValue
                    ? _bus.StopAsync(_stopTimeout.Value)
                    : _bus.StopAsync(cancellationToken)).ConfigureAwait(false);

                _stopped = true;
            }
        }
    }
}
