namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Registration;
    using Util;


    public class MassTransitHostedService :
        IHostedService,
        IDisposable
    {
        readonly IBusDepot _depot;
        readonly TimeSpan? _startTimeout;
        readonly TimeSpan? _stopTimeout;
        readonly bool _waitUntilStarted;
        Task _startTask;
        bool _stopped;

        public MassTransitHostedService(IBusDepot depot, bool waitUntilStarted, TimeSpan? startTimeout = null, TimeSpan? stopTimeout = null)
        {
            _depot = depot;
            _waitUntilStarted = waitUntilStarted;
            _startTimeout = startTimeout;
            _stopTimeout = stopTimeout;
        }

        public void Dispose()
        {
            if (_stopped)
                return;

            if (_stopTimeout.HasValue)
            {
                using var tokenSource = new CancellationTokenSource(_stopTimeout.Value);

                // ReSharper disable once AccessToDisposedClosure
                TaskUtil.Await(() => _depot.Stop(tokenSource.Token), tokenSource.Token);
            }
            else
                TaskUtil.Await(() => _depot.Stop(CancellationToken.None));

            _stopped = true;
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

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_stopped)
            {
                await (_stopTimeout.HasValue
                    ? _depot.Stop(_stopTimeout.Value)
                    : _depot.Stop(cancellationToken)).ConfigureAwait(false);

                _stopped = true;
            }
        }
    }
}
