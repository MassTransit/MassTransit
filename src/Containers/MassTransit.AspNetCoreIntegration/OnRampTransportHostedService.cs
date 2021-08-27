using MassTransit.Transports.OnRamp;
using MassTransit.Transports.OnRamp.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.AspNetCoreIntegration
{
    public class OnRampTransportHostedService : BackgroundService
    {
        private readonly OnRampInstanceState _state;
        private readonly IBusControl _busControl;
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private readonly IOnRampTransportOptions _onRampTransportOptions;

        public OnRampTransportHostedService(
            OnRampInstanceState state,
            IBusControl busControl,
            ILogger<OnRampTransportHostedService> logger,
            IOnRampTransportOptions onRampTransportOptions,
            IServiceProvider provider)
        {
            _state = state;
            _busControl = busControl;
            _provider = provider;
            _onRampTransportOptions = onRampTransportOptions;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OnRamp Transport Hosted Service is working.");

            await _state.Initialize();

            // Initialize the repository
            using (var scope = _provider.CreateScope())
            {
                var repositoryInitializer =
                    scope.ServiceProvider
                        .GetRequiredService<IRepositoryInitializer>();

                await repositoryInitializer.InitializeAsync(cancellationToken);
            }

            var tasks = new List<Task>(3);

            var busHealthCircuitBreaker = new BusHealthCircuitBreaker(_busControl);

            if (!_onRampTransportOptions.DisableServices)
            {
                tasks.Add(BusHealthChecker(busHealthCircuitBreaker, cancellationToken));
                tasks.Add(Sweeper(busHealthCircuitBreaker, cancellationToken));
                tasks.Add(Cluster(cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        #region BusHealthChecker
        private async Task BusHealthChecker(BusHealthCircuitBreaker circuitBreaker, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bus Health Check Service is running.");
            while (!cancellationToken.IsCancellationRequested)
            {
                circuitBreaker.PerformHealthCheck();

                await Task.Delay(5 * 1000, cancellationToken);
            }
        }
        #endregion BusHealthChecker

        #region Sweeper
        private readonly Guid _sweeperRequestorId = Guid.NewGuid();

        private async Task Sweeper(BusHealthCircuitBreaker circuitBreaker, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sweeper Service is running.");
            while (!cancellationToken.IsCancellationRequested)
            {
                var circuitBreakerCancellationToken = circuitBreaker.IsBusHealthy();
                // This will wait until the bus is considered healthy and we've had our first check in completed
                while (!cancellationToken.IsCancellationRequested && (circuitBreakerCancellationToken == null || _state.FirstCheckin))
                {
                    await Task.Delay(_onRampTransportOptions.SweeperPollingInterval, cancellationToken);
                    circuitBreakerCancellationToken = circuitBreaker.IsBusHealthy();
                }

                if (cancellationToken.IsCancellationRequested) break; // short circuit early if cancelled

                // We want the bus health check to be able to short circuit our sweeper, so we create LinkedTokenSource
                using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, circuitBreakerCancellationToken.Value);
                using var scope = _provider.CreateScope();

                var sweeperService =
                    scope.ServiceProvider
                        .GetRequiredService<ISweeperProcessor>();

                await sweeperService.ExecuteAsync(_sweeperRequestorId, combinedCts.Token).ConfigureAwait(false);

                await Task.Delay(_onRampTransportOptions.SweeperPollingInterval, cancellationToken).ConfigureAwait(false);
            }
        }
        #endregion Sweeper

        #region Cluster
        private int _numFails = 0;
        private readonly Guid _clusterRequestorId = Guid.NewGuid();
        private async Task Cluster(CancellationToken cancellationToken)
        {
            if (!_onRampTransportOptions.Clustered)
            {
                _logger.LogInformation("Clustering is off, cleaning up any jobs, and then stopping OnRamp Cluster Hosted Service.");

                using var scope = _provider.CreateScope();
                var clusterManager =
                    scope.ServiceProvider
                        .GetRequiredService<IClusterManager>();

                await clusterManager.RecoverMessagesAndCleanup(_clusterRequestorId, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation("OnRamp Cluster Hosted Service is working.");
                while (!cancellationToken.IsCancellationRequested)
                {

                    await CheckIn(cancellationToken).ConfigureAwait(false);

                    TimeSpan timeToSleep = _onRampTransportOptions.ClusterCheckinInterval;
                    TimeSpan transpiredTime = DateTimeOffset.UtcNow - _state.LastCheckin;
                    timeToSleep = timeToSleep - transpiredTime;
                    if (timeToSleep <= TimeSpan.Zero)
                    {
                        timeToSleep = TimeSpan.FromMilliseconds(100);
                    }

                    if (_numFails > 0)
                    {
                        timeToSleep = _onRampTransportOptions.ClusterDbRetryInterval > timeToSleep ? _onRampTransportOptions.ClusterDbRetryInterval : timeToSleep;
                    }

                    await Task.Delay(timeToSleep, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task CheckIn(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var clusterManager =
                    scope.ServiceProvider
                        .GetRequiredService<IClusterManager>();

                await clusterManager.CheckIn(_clusterRequestorId, cancellationToken).ConfigureAwait(false);

                _numFails = 0;
            }
            catch (Exception e)
            {
                if (_numFails % _onRampTransportOptions.ClusterRetryableActionErrorLogThreshold == 0)
                {
                    _logger.LogError(e, "Error managing cluster: " + e.Message);
                }
                _numFails++;
            }
        }
        #endregion Cluster

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OnRamp Transport Hosted Service is stopping.");

            await base.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public class BusHealthCircuitBreaker
    {
        private readonly object _padlock = new object();
        private readonly IBusControl _busControl;
        private bool _isBusHealthy;
        public CancellationTokenSource _cancellation;

        public BusHealthCircuitBreaker(IBusControl busControl)
        {
            _busControl = busControl;
            _isBusHealthy = false;
        }

        public CancellationToken? IsBusHealthy()
        {
            // We return back null if it's unhealthy, and back a Cancellation Token if it's healthy
            lock(_padlock)
            {
                if (_isBusHealthy) return _cancellation.Token;
                else return null;
            }
        }

        public void PerformHealthCheck()
        {
            lock (_padlock)
            {
                if (!_isBusHealthy && _busControl.CheckHealth().Status == BusHealthStatus.Healthy)
                {
                    _cancellation = new CancellationTokenSource();
                    _isBusHealthy = true;
                }
                else if (_isBusHealthy && _busControl.CheckHealth().Status != BusHealthStatus.Healthy)
                {
                    _cancellation.Cancel();
                    _isBusHealthy = false;
                }
            }
        }
    }
}
