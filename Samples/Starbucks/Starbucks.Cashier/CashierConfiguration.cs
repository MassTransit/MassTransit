using MassTransit.Host;
using MassTransit.Host.Configurations;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Cashier
{
    public class CashierConfiguration: LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifecycle;

        public CashierConfiguration(IServiceLocator serviceLocator)
        {
            _lifecycle = new CashierLifecycle(serviceLocator);
        }

        public override string[] Dependencies
        {
            get { return new[] { KnownServiceNames.Msmq }; }
        }

        public override string ServiceName
        {
            get { return "StarbucksCashier"; }
        }

        public override string DisplayName
        {
            get { return "Startbucks Cashier"; }
        }

        public override string Description
        {
            get { return "a Mass Transit sample service for handling orders of coffee."; }
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifecycle; }
        }
    }
}