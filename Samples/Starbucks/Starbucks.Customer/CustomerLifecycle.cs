namespace Starbucks.Customer
{
    using MassTransit.Host.Actions;
    using MassTransit.Host.LifeCycles;
    using Microsoft.Practices.ServiceLocation;

    public class CustomerLifecycle :
        HostedLifecycle
    {
        public CustomerLifecycle(IServiceLocator serviceLocator) : base(serviceLocator)
        {
        }

        public override NamedAction DefaultAction
        {
            get
            {
                return NamedAction.Gui;
            }
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}