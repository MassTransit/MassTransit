using System;
using Castle.Windsor;
using MassTransit.Host;
using MassTransit.WindsorIntegration;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Cashier
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Cashier.Castle.xml");
            var builder = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => builder);

            container.AddComponent<EmoCollegeDropout>();

            var credentials = Credentials.LocalSystem;
            var settings = WinServiceSettings.Custom(
                "StarbucksCashier",
                "Starbucks Cashier",
                "a Mass Transit sample service for handling orders of coffee.",
                KnownServiceNames.Msmq);
            var lifecycle = new CashierLifecycle(ServiceLocator.Current);
            
            Runner.Run(credentials, settings, lifecycle, args);
        }
    }
}