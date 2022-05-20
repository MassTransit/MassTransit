namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    class HealthResultReceiveEndpointObserver :
        IReceiveEndpointObserver
    {
        readonly ReceiveEndpoint _endpoint;

        public HealthResultReceiveEndpointObserver(ReceiveEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            if (_endpoint.CurrentState == ReceiveEndpoint.State.Initial
                || _endpoint.CurrentState == ReceiveEndpoint.State.Started
                || _endpoint.CurrentState == ReceiveEndpoint.State.Completed
                || _endpoint.CurrentState == ReceiveEndpoint.State.Faulted)
            {
                _endpoint.InputAddress = ready.InputAddress;
                _endpoint.Message = ready.IsStarted ? "ready" : "ready (not started)";
                _endpoint.HealthResult = EndpointHealthResult.Healthy(_endpoint, _endpoint.Message);

                LogContext.Debug?.Log("Endpoint Ready: {InputAddress}", _endpoint.InputAddress);

                _endpoint.CurrentState = ReceiveEndpoint.State.Ready;
            }

            return Task.CompletedTask;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            if (_endpoint.CurrentState == ReceiveEndpoint.State.Started
                || _endpoint.CurrentState == ReceiveEndpoint.State.Ready
                || _endpoint.CurrentState == ReceiveEndpoint.State.Completed
                || _endpoint.CurrentState == ReceiveEndpoint.State.Faulted)
            {
                _endpoint.Message = "stopping";

                LogContext.Debug?.Log("Endpoint Stopping: {InputAddress}", _endpoint.InputAddress);
            }

            return Task.CompletedTask;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            if (_endpoint.CurrentState == ReceiveEndpoint.State.Initial
                || _endpoint.CurrentState == ReceiveEndpoint.State.Started
                || _endpoint.CurrentState == ReceiveEndpoint.State.Ready
                || _endpoint.CurrentState == ReceiveEndpoint.State.Faulted)
            {
                _endpoint.InputAddress = completed.InputAddress;
                _endpoint.Message = $"stopped (delivered {completed.DeliveryCount} messages)";
                _endpoint.HealthResult = EndpointHealthResult.Degraded(_endpoint, _endpoint.Message);

                LogContext.Debug?.Log("Endpoint Completed: {InputAddress}", _endpoint.InputAddress);

                _endpoint.CurrentState = ReceiveEndpoint.State.Completed;
            }

            return Task.CompletedTask;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            if (_endpoint.CurrentState == ReceiveEndpoint.State.Initial
                || _endpoint.CurrentState == ReceiveEndpoint.State.Started
                || _endpoint.CurrentState == ReceiveEndpoint.State.Ready
                || _endpoint.CurrentState == ReceiveEndpoint.State.Completed)
            {
                _endpoint.InputAddress = faulted.InputAddress;
                _endpoint.Message = $"faulted ({faulted.Exception.Message})";
                _endpoint.HealthResult = EndpointHealthResult.Unhealthy(_endpoint, _endpoint.Message, faulted.Exception);

                LogContext.Debug?.Log(faulted.Exception, "Endpoint Faulted: {InputAddress}", _endpoint.InputAddress);

                _endpoint.CurrentState = ReceiveEndpoint.State.Faulted;
            }

            return Task.CompletedTask;
        }
    }
}
