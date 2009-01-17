namespace Starbucks.Barista
{
    using System;
    using System.IO;
    using Castle.Windsor;
    using log4net.Config;
    using MassTransit.Host;
    using MassTransit.Saga;
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
            XmlConfigurator.Configure(new FileInfo("barista.log4net.xml"));
            IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Barista.Castle.xml");
            var builder = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => builder);

            container.AddComponent<DrinkPreparationSaga>();
            container.AddComponent<ISagaRepository<DrinkPreparationSaga>, InMemorySagaRepository<DrinkPreparationSaga>>();

            Credentials credentials = Credentials.Interactive;
            WinServiceSettings settings = WinServiceSettings.Custom(
                "StarbucksBarista",
                "Starbucks Barista",
                "a Mass Transit sample service for making orders of coffee.",
                KnownServiceNames.Msmq);
            var lifecycle = new BaristaLifecycle(ServiceLocator.Current);

            Runner.Run(credentials, settings, lifecycle, args);
        }
    }
}