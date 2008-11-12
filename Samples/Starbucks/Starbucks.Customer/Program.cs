using System;
using System.Windows.Forms;
using Castle.Windsor;
using MassTransit.Host;
using MassTransit.WindsorIntegration;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Customer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Customer.Castle.xml");
            var builder = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => builder);

            container.AddComponent<Form, OrderDrinkForm>();

            //do container Stuff

            var credentials = Credentials.LocalSystem;
            var settings = WinServiceSettings.Custom(
                "StarbucksCustomer",
                "Starbucks Customer",
                "a Mass Transit sample service for ordering coffee.",
                KnownServiceNames.Msmq);
            var lifecycle = new CustomerLifecycle(ServiceLocator.Current);
            
            Runner.Run(credentials, settings, lifecycle, args);
        }
    }
}
