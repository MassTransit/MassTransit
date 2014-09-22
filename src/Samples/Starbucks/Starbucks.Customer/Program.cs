namespace Starbucks.Customer
{
    using System;
    using System.Windows.Forms;
    using MassTransit;
    using MassTransit.Log4NetIntegration.Logging;
    using StructureMap;


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log4NetLogger.Use("customer.log4net.xml");

            IContainer c = BootstrapContainer();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OrderDrinkForm(c.GetInstance<IServiceBus>()));
        }

        static IContainer BootstrapContainer()
        {
            IContainer container = new Container(x => x.AddType(typeof(OrderDrinkForm), typeof(OrderDrinkForm)));

            container.Configure(cfg =>
            {
                cfg.For<IServiceBus>().Use(context => CreateServiceBus(container));
            });

            return container;
        }

        static IServiceBus CreateServiceBus(IContainer container)
        {
            return ServiceBusFactory.New(sbc =>
            {
                sbc.ReceiveFrom("msmq://localhost/starbucks_customer");
                sbc.UseMsmq();
                sbc.UseMulticastSubscriptionClient();

                sbc.UseControlBus();

                sbc.Subscribe(subs => subs.LoadFrom(container));
            });
        }
    }
}