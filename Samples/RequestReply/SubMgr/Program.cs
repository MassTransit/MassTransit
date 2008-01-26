namespace SubMgr
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            MassTransit.ServiceBus.SubscriptionsManager.Program pgm = new MassTransit.ServiceBus.SubscriptionsManager.Program();
            pgm.StartItUp();

            Console.WriteLine("Started...");
            Console.ReadLine();
        }
    }
}
