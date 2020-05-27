namespace MassTransit.Monitoring.Health
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Attachments;
    using EndpointConfigurators;
    using Util;


    public class BusHealth :
        IBusObserver,
        IEndpointConfigurationObserver,
        IBusHealth
    {
        readonly ConcurrentDictionary<string, BusAttachmentHealth> _attachmentsHealth;
        readonly EndpointHealth _endpointHealth;

        string _failureMessage = "not started";
        bool _healthy;

        public BusHealth(string name)
        {
            Name = name;
            _endpointHealth = new EndpointHealth();
            _attachmentsHealth = new ConcurrentDictionary<string, BusAttachmentHealth>();
        }

        public string Name { get; }

        public HealthResult CheckHealth()
        {
            var endpointHealthResult = _endpointHealth.CheckHealth();
            Dictionary<string, HealthResult> attachmentHealthResult = _attachmentsHealth
                .ToDictionary(x => x.Key, x => x.Value.CheckHealth());

            var data = new Dictionary<string, object>
            {
                ["Attachments"] = attachmentHealthResult.ToDictionary(x => x.Key, x => x.Value.Description),
                ["Endpoints"] = endpointHealthResult.Data
            };

            return _healthy && endpointHealthResult.Status == BusHealthStatus.Healthy
                && attachmentHealthResult.Values.All(x => x.Status == BusHealthStatus.Healthy)
                    ? HealthResult.Healthy("Ready", data)
                    : HealthResult.Unhealthy($"Not ready: {_failureMessage}", data: data);
        }

        Task IBusObserver.CreateFaulted(Exception exception)
        {
            return Failure($"create faulted: {exception.Message}");
        }

        Task IBusObserver.PostCreate(IBus bus)
        {
            bus.ConnectReceiveEndpointObserver(_endpointHealth);

            return TaskUtil.Completed;
        }

        Task IBusObserver.PostStart(IBus bus, Task<BusReady> busReady)
        {
            return Success();
        }

        Task IBusObserver.StartFaulted(IBus bus, Exception exception)
        {
            return Failure($"start faulted: {exception.Message}");
        }

        Task IBusObserver.PreStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        Task IBusObserver.PostStop(IBus bus)
        {
            return Failure("stopped");
        }

        Task IBusObserver.StopFaulted(IBus bus, Exception exception)
        {
            return Failure($"stop faulted: {exception.Message}");
        }

        Task IBusObserver.PreStart(IBus bus)
        {
            return TaskUtil.Completed;
        }

        void IEndpointConfigurationObserver.EndpointConfigured<T>(T configurator)
        {
            _endpointHealth.EndpointConfigured(configurator);
        }

        public void Configure(IBusAttachmentFactoryConfigurator configurator)
        {
            var health = _attachmentsHealth.GetOrAdd(configurator.Name, n => new BusAttachmentHealth());
            configurator.ConnectBusAttachmentObserver(health);
            configurator.ConnectReceiveEndpointObserver(_endpointHealth);
        }

        Task Failure(string message)
        {
            _healthy = false;
            _failureMessage = message;

            return TaskUtil.Completed;
        }

        Task Success()
        {
            _healthy = true;
            _failureMessage = "";

            return TaskUtil.Completed;
        }
    }
}
