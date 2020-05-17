namespace MassTransit.Testing.Observers
{
    using System.Threading.Tasks;
    using Context;
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
            LogContext.Debug?.Log("Endpoint Ready: {InputAddress}", ready.InputAddress);

            ready.ReceiveEndpoint.ConnectPublishObserver(_publishObserver);

            return TaskUtil.Completed;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return TaskUtil.Completed;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            LogContext.Debug?.Log("Endpoint Complete: {DeliveryCount}/{ConcurrentDeliveryCount} {InputAddress}", completed.DeliveryCount,
                completed.ConcurrentDeliveryCount, completed.InputAddress);

            return TaskUtil.Completed;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            LogContext.Debug?.Log("Endpoint Faulted: {Exception} {InputAddress}", faulted.Exception, faulted.InputAddress);

            return TaskUtil.Completed;
        }
    }
}
