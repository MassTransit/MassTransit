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
            
            IInstallationConfiguration config = new CashierConfiguration(ServiceLocator.Current);
            Runner.Run(config, args);
        }
    }
}