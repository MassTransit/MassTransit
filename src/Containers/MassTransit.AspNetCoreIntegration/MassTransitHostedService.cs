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
        readonly TimeSpan? _startTimeout;
        readonly TimeSpan? _stopTimeout;
        readonly bool _waitUntilStarted;
        Task _startTask;

        public MassTransitHostedService(IBusDepot depot, bool waitUntilStarted, TimeSpan? startTimeout = null, TimeSpan? stopTimeout = null)
        {
            _depot = depot;
            _waitUntilStarted = waitUntilStarted;
            _startTimeout = startTimeout;
            _stopTimeout = stopTimeout;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask = _startTimeout.HasValue
                ? _depot.Start(_startTimeout.Value)
                : _depot.Start(cancellationToken);

            return _startTask.IsCompleted || _waitUntilStarted
                ? _startTask
                : TaskUtil.Completed;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _stopTimeout.HasValue
                ? _depot.Stop(_stopTimeout.Value)
                : _depot.Stop(cancellationToken);
        }
    }
}
