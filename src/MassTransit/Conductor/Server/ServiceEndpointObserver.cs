namespace MassTransit.Conductor.Server
{
    using System.Threading.Tasks;
    using Util;


    public class ServiceEndpointObserver :
        IReceiveEndpointObserver
    {
        readonly ServiceEndpoint _serviceEndpoint;

        public ServiceEndpointObserver(ServiceEndpoint serviceEndpoint)
        {
            _serviceEndpoint = serviceEndpoint;
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            return _serviceEndpoint.NotifyEndpointReady(ready.ReceiveEndpoint);
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return _serviceEndpoint.NotifyDown();
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
