namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Util;


    public class MassTransitHostedService :
        IHostedService
    {
        readonly IBusControl _bus;
        readonly SimplifiedBusHealthCheck _simplifiedBusCheck;
        readonly ReceiveEndpointHealthCheck _receiveEndpointCheck;
        Task<BusHandle> _startTask;

        [Obsolete("Use the constructor that doesn't require SimplifiedBusHealthCheck")]
        public MassTransitHostedService(IBusControl bus, SimplifiedBusHealthCheck simplifiedBusCheck, ReceiveEndpointHealthCheck receiveEndpointCheck)
        {
            _bus = bus;
            _simplifiedBusCheck = simplifiedBusCheck;
            _receiveEndpointCheck = receiveEndpointCheck;
        }

        public MassTransitHostedService(IBusControl bus, ReceiveEndpointHealthCheck receiveEndpointCheck)
        {
            _bus = bus;
            _receiveEndpointCheck = receiveEndpointCheck;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _bus.ConnectReceiveEndpointObserver(_receiveEndpointCheck);

            _startTask = _bus.StartAsync(cancellationToken);

            if (_startTask.IsCompleted)
            {
                if (_startTask.IsCompletedSuccessfully())
                    _simplifiedBusCheck?.ReportBusStarted();

                return _startTask;
            }

            if (_simplifiedBusCheck != null)
                _startTask.ContinueWith(task => _simplifiedBusCheck?.ReportBusStarted(), TaskContinuationOptions.OnlyOnRanToCompletion);

            return TaskUtil.Completed;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bus.StopAsync(cancellationToken).ConfigureAwait(false);

            _simplifiedBusCheck?.ReportBusStopped();
        }
    }
}
