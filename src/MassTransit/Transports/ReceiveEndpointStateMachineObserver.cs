namespace MassTransit.Transports
{
    using System.Threading.Tasks;
    using Automatonymous;


    class ReceiveEndpointStateMachineObserver :
        IReceiveEndpointObserver
    {
        readonly ReceiveEndpointStateMachine _machine;
        readonly ReceiveEndpoint _endpoint;

        public ReceiveEndpointStateMachineObserver(ReceiveEndpointStateMachine machine, ReceiveEndpoint endpoint)
        {
            _machine = machine;
            _endpoint = endpoint;
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            return _machine.RaiseEvent(_endpoint, x => x.ReceiveEndpointReady, ready);
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return _machine.RaiseEvent(_endpoint, x => x.ReceiveEndpointStopping, stopping);
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            return _machine.RaiseEvent(_endpoint, x => x.ReceiveEndpointCompleted, completed);
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            return _machine.RaiseEvent(_endpoint, x => x.ReceiveEndpointFaulted, faulted);
        }
    }
}
