using MassTransit.Host;
using MassTransit.Host.Configurations;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Barista
{
    public class BaristaConfiguration : LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifecycle;

        public BaristaConfiguration(IServiceLocator serviceLocator)
        {
            _lifecycle = new BaristaLifecycle(serviceLocator);
        }

        public override string[] Dependencies
        {
            get { return new[] { KnownServiceNames.Msmq }; }
        }

        public override string ServiceName
        {
            get { return "StarbucksBarista"; }
        }

        public override string DisplayName
        {
            get { return "Startbucks Barista"; }
        }

        public override string Description
        {
            get { return "a Mass Transit sample service for making orders of coffee."; }
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifecycle; }
        }
    }
}