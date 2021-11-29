namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;


    public class MassTransitHostedService :
        IHostedService,
        IAsyncDisposable
    {
        readonly IBusDepot _depot;
        readonly IOptions<MassTransitHostOptions> _options;
        Task _startTask;
        bool _stopped;

        public MassTransitHostedService(IBusDepot depot, IOptions<MassTransitHostOptions> options)
        {
            _depot = depot;
            _options = options;
        }

        public async ValueTask DisposeAsync()
        {
            if (_stopped)
                return;

            if (_options.Value.StopTimeout.HasValue)
            {
                using var tokenSource = new CancellationTokenSource(_options.Value.StopTimeout.Value);

                await _depot.Stop(tokenSource.Token).ConfigureAwait(false);
            }
            else
                await _depot.Stop(CancellationToken.None).ConfigureAwait(false);

            _stopped = true;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask = _options.Value.StartTimeout.HasValue
                ? _depot.Start(_options.Value.StartTimeout.Value, cancellationToken)
                : _depot.Start(cancellationToken);

            return _startTask.IsCompleted || _options.Value.WaitUntilStarted
                ? _startTask
                : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_stopped)
            {
                await (_options.Value.StopTimeout.HasValue
                    ? _depot.Stop(_options.Value.StopTimeout.Value, cancellationToken)
                    : _depot.Stop(cancellationToken)).ConfigureAwait(false);

                _stopped = true;
            }
        }
    }
}
