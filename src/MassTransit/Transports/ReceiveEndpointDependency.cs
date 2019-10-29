namespace MassTransit.Transports
{
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Util;


    public class ReceiveEndpointDependency :
        IReceiveEndpointDependency,
        IReceiveEndpointObserver
    {
        readonly TaskCompletionSource<ReceiveEndpointReady> _ready;
        ConnectHandle _handle;

        public ReceiveEndpointDependency(IReceiveEndpointObserverConnector connector)
        {
            _ready = TaskUtil.GetTask<ReceiveEndpointReady>();

            _handle = connector.ConnectReceiveEndpointObserver(this);
        }

        public Task Ready => _ready.Task;

        Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
        {
            _handle.Disconnect();

            return _ready.TrySetResultOnThreadPool(ready);
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
            _handle.Disconnect();

            return _ready.TrySetExceptionOnThreadPool(faulted.Exception);
        }
    }
}
