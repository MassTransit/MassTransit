namespace MassTransit.AspNetCoreIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using HealthChecks;
    using Logging.Tracing;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;


    public class MassTransitHostedService :
        IHostedService
    {
        readonly IBusControl _bus;
        readonly SimplifiedBusHealthCheck _simplifiedBusCheck;
        readonly ReceiveEndpointHealthCheck _receiveEndpointCheck;

        public MassTransitHostedService(IBusControl bus, ILoggerFactory loggerFactory, SimplifiedBusHealthCheck simplifiedBusCheck,
            ReceiveEndpointHealthCheck receiveEndpointCheck)
        {
            _bus = bus;
            _simplifiedBusCheck = simplifiedBusCheck;
            _receiveEndpointCheck = receiveEndpointCheck;

            if (loggerFactory != null && Logging.Logger.Current.GetType() == typeof(TraceLogger))
                ExtensionsLoggingIntegration.ExtensionsLogger.Use(loggerFactory);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _bus.ConnectReceiveEndpointObserver(_receiveEndpointCheck);

            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);

            _simplifiedBusCheck.ReportBusStarted();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _bus.StopAsync(cancellationToken);
        }
    }
}
