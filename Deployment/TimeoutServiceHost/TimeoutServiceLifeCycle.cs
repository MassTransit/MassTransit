namespace TimeoutServiceHost
{
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Timeout;

    public class TimeoutServiceLifeCycle : 
        HostedLifeCycle

    {
        public TimeoutServiceLifeCycle(string xmlFile) : base(xmlFile)
        {
            
        }

        public override void Start()
        {
            Container.AddComponent<ITimeoutRepository, InMemoryTimeoutRepository>();
            Container.AddComponent<IHostedService, TimeoutService>();
        }

        public override void Stop()
        {
            
        }
    }
}