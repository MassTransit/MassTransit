namespace HealthServiceHost
{
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.HealthMonitoring;

    public class HealthLifeCycle :
        HostedLifeCycle
    {
        public HealthLifeCycle(string xmlFile) : base(xmlFile)
        {
        }

        public override void Start()
        {
            Container.AddComponent<IHostedService, HealthService>();
            Container.AddComponent<IHealthCache, LocalHealthCache>();
        }

        public override void Stop()
        {
            
        }
    }
}