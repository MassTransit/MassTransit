namespace MassTransit.Monitoring
{
    using System.Threading.Tasks;


    public class InstrumentReceiveEndpointObserver :
        IReceiveEndpointObserver
    {
        public Task Ready(ReceiveEndpointReady ready)
        {
            Instrumentation.EndpointReady(ready);

            return Task.CompletedTask;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return Task.CompletedTask;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            Instrumentation.EndpointCompleted(completed);

            return Task.CompletedTask;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            return Task.CompletedTask;
        }
    }
}
