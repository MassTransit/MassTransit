namespace MassTransit.PrometheusIntegration
{
    using System.Threading.Tasks;


    public class PrometheusReceiveEndpointObserver :
        IReceiveEndpointObserver
    {
        public Task Ready(ReceiveEndpointReady ready)
        {
            PrometheusMetrics.EndpointReady(ready);

            return Task.CompletedTask;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return Task.CompletedTask;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            PrometheusMetrics.EndpointCompleted(completed);

            return Task.CompletedTask;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            return Task.CompletedTask;
        }
    }
}
