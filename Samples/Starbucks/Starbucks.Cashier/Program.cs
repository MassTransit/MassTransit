namespace Starbucks.Cashier
{
    using System;
    using System.IO;
    using Castle.Windsor;
    using log4net.Config;
    using MassTransit.Host;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("cashier.log4net.xml"));
            IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Cashier.Castle.xml");
            var builder = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => builder);

            container.AddComponent<FriendlyCashier>();

            Credentials credentials = Credentials.LocalSystem;
            WinServiceSettings settings = WinServiceSettings.Custom(
                "StarbucksCashier",
                "Starbucks Cashier",
                "a Mass Transit sample service for handling orders of coffee.",
                KnownServiceNames.Msmq);
            var lifecycle = new CashierLifecycle(ServiceLocator.Current);

            Runner.Run(credentials, settings, lifecycle, args);
        }
    }
}