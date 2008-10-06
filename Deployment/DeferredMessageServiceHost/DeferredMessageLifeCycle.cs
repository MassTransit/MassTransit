namespace DeferredMessageServiceHost
{
    using MassTransit.Host.LifeCycles;

    public class DeferredMessageLifeCycle :
        HostedLifeCycle
    {
        public DeferredMessageLifeCycle(string xmlFile) : base(xmlFile)
        {
        }

        public override void Start()
        {
            
        }

        public override void Stop()
        {
            
        }
    }
}