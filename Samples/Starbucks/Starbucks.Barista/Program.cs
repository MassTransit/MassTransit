using System;
using Castle.Windsor;
using MassTransit.Host;
using MassTransit.Saga;
using MassTransit.WindsorIntegration;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Barista
{
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

            IInstallationConfiguration config = new BaristaConfiguration(ServiceLocator.Current);
            Runner.Run(config, args);
        }
    }
}