namespace MassTransit.ServiceBusIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using Microsoft.Extensions.Logging;


    public class AsyncBusHandle :
        IAsyncBusHandle
    {
        readonly CancellationTokenSource _cancellationToken;
        readonly Task<BusHandle> _handleTask;
        readonly ILogger<MassTransitBus> _logger;

        public AsyncBusHandle(IBusControl busControl, ILogger<MassTransitBus> logger)
        {
            _logger = logger;
            _cancellationToken = new CancellationTokenSource();

            _logger.LogInformation("Starting MassTransit");
            _handleTask = busControl.StartAsync(_cancellationToken.Token);
        }

        public void Dispose()
        {
            if (_handleTask.IsCompletedSuccessfully())
            {
                _logger.LogInformation("Stopping MassTransit (disposed)");
                _handleTask.GetAwaiter().GetResult().StopAsync();
            }
            else
            {
                _logger.LogInformation("Canceling MassTransit Start (disposed)");

                _cancellationToken.Cancel();
            }
        }
    }
}
