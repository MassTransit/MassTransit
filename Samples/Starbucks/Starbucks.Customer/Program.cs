namespace Starbucks.Customer
{
    using System;
    using System.Windows.Forms;
    using Castle.Windsor;
    using MassTransit.WindsorIntegration;
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
            IRunConfiguration cfg = RunnerConfigurator.New(c =>
                                                               {
                                                                   c.SetServiceName("StarbucksCustomer");
                                                                   c.SetDisplayName("Starbucks Customer");
                                                                   c.SetDescription(
                                                                       "a Mass Transit sample service for ordering coffee.");

                                                                   c.RunAsLocalSystem();
                                                                   c.DependencyOnMsmq();
                                                                   c.UseWinFormHost<OrderDrinkForm>();

                                                                   c.BeforeStart(a =>
                                                                                     {
                                                                                         IWindsorContainer container =
                                                                                             new DefaultMassTransitContainer
                                                                                                 ("Starbucks.Customer.Castle.xml");
                                                                                         container.AddComponent
                                                                                             <Form, OrderDrinkForm>();
                                                                                     });
                                                                   c.ConfigureService<CustomerLifecycle>();
                                                               });
            Runner.Host(cfg, args);
        }
    }
}