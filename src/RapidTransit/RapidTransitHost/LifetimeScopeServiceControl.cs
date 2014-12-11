namespace RapidTransit
{
    using Autofac;
    using Topshelf;


    /// <summary>
    /// Decorates ServiceControl and manages the LifetimeScope for the service as part of the Start/Stop handshake.
    /// If the service is stopped, the LifetimeScope is disposed. The service cannot be restarted.
    /// </summary>
    public class LifetimeScopeServiceControl :
        ServiceControl
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly ServiceControl _service;
        readonly string _serviceName;

        public LifetimeScopeServiceControl(ILifetimeScope lifetimeScope, ServiceControl service, string serviceName)
        {
            _lifetimeScope = lifetimeScope;
            _service = service;
            _serviceName = serviceName;
        }

        public bool Start(HostControl hostControl)
        {
            return _service.Start(hostControl);
        }

        public bool Stop(HostControl hostControl)
        {
            bool stop = _service.Stop(hostControl);
            if (stop)
                _lifetimeScope.Dispose();
            return stop;
        }

        public override string ToString()
        {
            return _serviceName;
        }
    }
}