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
        readonly bool _waitUntilStarted;
        private readonly TimeSpan _startTimeout;
        private readonly TimeSpan _stopTimeout;
        Task _startTask;

        public BusHostedService(IBusControl bus, bool waitUntilStarted, TimeSpan startTimeout = default, TimeSpan stopTimeout = default)
        {
            _bus = bus;
            _waitUntilStarted = waitUntilStarted;
            _startTimeout = startTimeout;
            _stopTimeout = stopTimeout;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask =
                _startTimeout == default
                ? _bus.StartAsync(cancellationToken)
                : _bus.StartAsync(_startTimeout);

            return _startTask.IsCompleted || _waitUntilStarted
                ? _startTask
                : TaskUtil.Completed;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return
                _stopTimeout == default
                ? _bus.StopAsync(cancellationToken)
                : _bus.StopAsync(_stopTimeout);
        }
    }
}
