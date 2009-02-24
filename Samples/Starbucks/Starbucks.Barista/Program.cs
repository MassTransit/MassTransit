namespace Starbucks.Barista
{
    using System;
    using System.IO;
    using Castle.Windsor;
    using log4net.Config;
    using MassTransit.Saga;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;
    using Topshelf;
    using Topshelf.Configuration;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("barista.log4net.xml"));
            
            var cfg = RunnerConfigurator.New(c =>
                                                 {
                                                     c.SetServiceName("StarbucksBarista");
                                                     c.SetDisplayName("Starbucks Barista");
                                                     c.SetDescription("a Mass Transit sample service for making orders of coffee.");

                                                     c.DependencyOnMsmq();
                                                     c.RunAsFromInteractive();

                                                     c.BeforeStart(a=>
                                                                       {
                                                                           IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Barista.Castle.xml");
                                                                           var builder = new WindsorObjectBuilder(container.Kernel);
                                                                           ServiceLocator.SetLocatorProvider(() => builder);

                                                                           container.AddComponent<DrinkPreparationSaga>();
                                                                           container.AddComponent<BaristaService>();
                                                                           container.AddComponent<ISagaRepository<DrinkPreparationSaga>, InMemorySagaRepository<DrinkPreparationSaga>>();
                                                                       });

                                                     c.ConfigureService<BaristaService>(s=>
                                                                                              {
                                                                                                  s.WhenStarted(o=>o.Start());
                                                                                                  s.WhenStopped(o=>o.Stop());
                                                                                              });
                                                 });
            Runner.Host(cfg, args);
        }
    }
}