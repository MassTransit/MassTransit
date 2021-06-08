namespace MassTransit.Transports
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public class ReceiveEndpointDependency :
        IReceiveEndpointDependency,
        IReceiveEndpointObserver
    {
        readonly ConnectHandle _handle;
        readonly TaskCompletionSource<ReceiveEndpointReady> _ready;

        public ReceiveEndpointDependency(IReceiveEndpointObserverConnector connector)
        {
            _ready = TaskUtil.GetTask<ReceiveEndpointReady>();

            _handle = connector.ConnectReceiveEndpointObserver(this);
        }

        public Task Ready => _ready.Task;

        Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
        {
            _handle.Disconnect();

            _ready.TrySetResult(ready);

            return TaskUtil.Completed;
        }

        Task IReceiveEndpointObserver.Stopping(ReceiveEndpointStopping stopping)
        {
            return TaskUtil.Completed;
        }

        Task IReceiveEndpointObserver.Completed(ReceiveEndpointCompleted completed)
        {
            return TaskUtil.Completed;
        }

        Task IReceiveEndpointObserver.Faulted(ReceiveEndpointFaulted faulted)
        {
            return TaskUtil.Completed;
        }
    }
}
