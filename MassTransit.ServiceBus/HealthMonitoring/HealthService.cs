namespace MassTransit.ServiceBus.HealthMonitoring
{
    public class HealthService :
        IHostedService
    {
        private readonly IServiceBus _bus;


        public HealthService(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
            _bus.AddComponent<HeartbeatMonitor>();
            _bus.AddComponent<Investigator>();
            _bus.AddComponent<Reporter>();
        }

        public void Stop()
        {
            _bus.RemoveComponent<Reporter>();
            _bus.RemoveComponent<Investigator>();
            _bus.RemoveComponent<HeartbeatMonitor>();
        }

        public void Dispose()
        {
            //nothing yet
        }
    }
}