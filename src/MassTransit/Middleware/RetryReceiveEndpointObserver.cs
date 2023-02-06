namespace MassTransit.Middleware
{
    using System.Threading;
    using System.Threading.Tasks;


    public class RetryReceiveEndpointObserver :
        IReceiveEndpointObserver
    {
        readonly CancellationTokenSource _stopping;

        public RetryReceiveEndpointObserver()
        {
            _stopping = new CancellationTokenSource();
        }

        public CancellationToken Stopping => _stopping.Token;

        public Task Ready(ReceiveEndpointReady ready)
        {
            return Task.CompletedTask;
        }

        Task IReceiveEndpointObserver.Stopping(ReceiveEndpointStopping stopping)
        {
            _stopping.Cancel();

            return Task.CompletedTask;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            return Task.CompletedTask;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            return Task.CompletedTask;
        }
    }
}
