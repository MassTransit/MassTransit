namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using HealthChecks;
    using Microsoft.Extensions.Hosting;


    public class MassTransitHostedService :
        IHostedService
    {
        readonly IBusControl _bus;
        readonly SimplifiedBusHealthCheck _simplifiedBusCheck;
        readonly ReceiveEndpointHealthCheck _receiveEndpointCheck;

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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _bus.ConnectReceiveEndpointObserver(_receiveEndpointCheck);

            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);

            _simplifiedBusCheck?.ReportBusStarted();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bus.StopAsync(cancellationToken).ConfigureAwait(false);

            _simplifiedBusCheck?.ReportBusStopped();
        }
    }
}
