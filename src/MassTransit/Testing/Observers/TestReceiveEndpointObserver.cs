namespace MassTransit.Testing.Observers
{
    using System.Threading.Tasks;
    using Util;


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

            return TaskUtil.Completed;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return TaskUtil.Completed;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            return TaskUtil.Completed;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            return TaskUtil.Completed;
        }
    }
}
