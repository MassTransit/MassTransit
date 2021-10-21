namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Registration;
    using Util;


    public class MassTransitHostedService :
        IHostedService
    {
        readonly IBusDepot _depot;
        readonly bool _waitUntilStarted;
        private readonly TimeSpan _startTimeout;
        private readonly TimeSpan _stopTimeout;
        Task _startTask;

        public MassTransitHostedService(IBusDepot depot, bool waitUntilStarted, TimeSpan startTimeout = default, TimeSpan stopTimeout = default)
        {
            _depot = depot;
            _waitUntilStarted = waitUntilStarted;
            _startTimeout = startTimeout;
            _stopTimeout = stopTimeout;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask =
                _startTimeout == default
                ? _depot.Start(cancellationToken)
                : _depot.Start(_startTimeout);

            return _startTask.IsCompleted || _waitUntilStarted
                ? _startTask
                : TaskUtil.Completed;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return
                _stopTimeout == default
                ? _depot.Stop(cancellationToken)
                : _depot.Stop(_stopTimeout);
        }
    }
}
