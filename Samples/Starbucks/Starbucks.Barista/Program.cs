using System;
using Castle.Windsor;
using MassTransit.Host;
using MassTransit.Saga;
using MassTransit.WindsorIntegration;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Barista
{
    using MassTransit.Host.Configurations;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Barista.Castle.xml");
            var builder = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => builder);

            container.AddComponent<DrinkPreparationSaga>();
            container.AddComponent<ISagaRepository<DrinkPreparationSaga>, DrinkPreparationSagaRepository>();

            var credentials = Credentials.Interactive;
            var settings = WinServiceSettings.Custom(
                "StarbucksBarista",
                "Starbucks Barista",
                "a Mass Transit sample service for making orders of coffee.",
                KnownServiceNames.Msmq);
            var lifecycle = new BaristaLifecycle(ServiceLocator.Current);

            Runner.Run(credentials, settings, lifecycle, args);
        }
    }
}