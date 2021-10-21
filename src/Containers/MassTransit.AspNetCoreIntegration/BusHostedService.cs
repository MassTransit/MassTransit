namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Util;


    public class BusHostedService :
        IHostedService
    {
        readonly IBusControl _bus;
        readonly TimeSpan? _startTimeout;
        readonly TimeSpan? _stopTimeout;
        readonly bool _waitUntilStarted;
        Task _startTask;

        public BusHostedService(IBusControl bus, bool waitUntilStarted, TimeSpan? startTimeout = null, TimeSpan? stopTimeout = null)
        {
            _bus = bus;
            _waitUntilStarted = waitUntilStarted;
            _startTimeout = startTimeout;
            _stopTimeout = stopTimeout;
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _stopTimeout.HasValue
                ? _bus.StopAsync(_stopTimeout.Value)
                : _bus.StopAsync(cancellationToken);
        }
    }
}
