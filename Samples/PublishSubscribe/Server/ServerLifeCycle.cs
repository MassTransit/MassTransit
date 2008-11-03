namespace Server
{
    using MassTransit.Host.LifeCycles;
    using Microsoft.Practices.ServiceLocation;

    public class ServerLifeCycle :
        HostedLifecycle
    {
        public ServerLifeCycle(IServiceLocator serviceLocator) : base(serviceLocator)
        {
        }

        public override void Start()
        {
            //do nothing
        }

        public override void Stop()
        {
            //do nothing
        }
    }
}