namespace MassTransit.Testing.Implementations
{
    using System.Threading.Tasks;


    public class TestReceiveEndpointObserver :
        IReceiveEndpointObserver
    {
        readonly IPublishObserver _publishObserver;

        public TestReceiveEndpointObserver(IPublishObserver publishObserver)
        {
            _publishObserver = publishObserver;
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            ready.ReceiveEndpoint.ConnectPublishObserver(_publishObserver);

            return Task.CompletedTask;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
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
