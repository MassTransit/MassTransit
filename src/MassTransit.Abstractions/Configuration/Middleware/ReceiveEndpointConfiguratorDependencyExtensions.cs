namespace MassTransit
{
    using System.Threading.Tasks;
    using Transports;


    public static class ReceiveEndpointConfiguratorDependencyExtensions
    {
        public static void AddDependency(this IReceiveEndpointConfigurator connector, IReceiveEndpointConfigurator dependency)
        {
            connector.AddDependency(new ReceiveEndpointDependency(dependency));
            dependency.AddDependent(new ReceiveEndpointDependent(connector));
        }


        class ReceiveEndpointDependency :
            IReceiveEndpointDependency,
            IReceiveEndpointObserver
        {
            readonly ConnectHandle _handle;
            readonly TaskCompletionSource<ReceiveEndpointReady> _ready;

            public ReceiveEndpointDependency(IReceiveEndpointObserverConnector connector)
            {
                _ready = new TaskCompletionSource<ReceiveEndpointReady>(TaskCreationOptions.RunContinuationsAsynchronously);

                _handle = connector.ConnectReceiveEndpointObserver(this);
            }

            public Task Ready => _ready.Task;

            Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
            {
                _handle.Disconnect();

                _ready.TrySetResult(ready);

                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Stopping(ReceiveEndpointStopping stopping)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Completed(ReceiveEndpointCompleted completed)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Faulted(ReceiveEndpointFaulted faulted)
            {
                return Task.CompletedTask;
            }
        }


        class ReceiveEndpointDependent :
            IReceiveEndpointDependent,
            IReceiveEndpointObserver
        {
            readonly TaskCompletionSource<ReceiveEndpointCompleted> _completed;
            readonly ConnectHandle _handle;

            public ReceiveEndpointDependent(IReceiveEndpointObserverConnector connector)
            {
                _completed = new TaskCompletionSource<ReceiveEndpointCompleted>(TaskCreationOptions.RunContinuationsAsynchronously);

                _handle = connector.ConnectReceiveEndpointObserver(this);
            }

            public Task Completed => _completed.Task;

            Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Stopping(ReceiveEndpointStopping stopping)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Completed(ReceiveEndpointCompleted completed)
            {
                _handle.Disconnect();

                _completed.TrySetResult(completed);

                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Faulted(ReceiveEndpointFaulted faulted)
            {
                return Task.CompletedTask;
            }
        }
    }
}
