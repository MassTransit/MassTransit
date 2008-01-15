namespace SubMgr
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            MassTransit.ServiceBus.SubscriptionsManager.Program pgm = new MassTransit.ServiceBus.SubscriptionsManager.Program();
            pgm.StartItUp();

            Console.WriteLine("Started...");
            Console.ReadLine();
        }
    }
}
