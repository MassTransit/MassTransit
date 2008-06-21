namespace OpenAllNight
{
    using System;
    using System.IO;
    using System.Threading;
    using Castle.Windsor;
    using log4net.Config;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions.Messages;

    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            WindsorContainer c = new OpenAllNightContainer("castle.xml");

            IEndpoint ep = c.Resolve<IEndpointResolver>().Resolve(new Uri("msmq://localhost/mt_pubsub"));

            Console.WriteLine("Please enter the number of hours you would like this test to run for?");
            string input = Console.ReadLine();
            int hours = int.Parse(input);
            DateTime stopTime = DateTime.Now.AddHours(hours);

            Console.WriteLine("Test Started");

            while (DateTime.Now < stopTime )
            {
                ep.Send(new CacheUpdateRequest(new Uri("msmq://localhost/test_servicebus")));
                Thread.Sleep(1000);
                PrintTime();
            }
            
        }

        private static DateTime lastPrint = DateTime.Now;
        private static int tenMinuteIncrement = 0;

        private static void PrintTime()
        {
            TimeSpan ts = lastPrint.Subtract(DateTime.Now);
            if(ts.Minutes >= 10)
            {
                tenMinuteIncrement++;
                Console.WriteLine("Test has been running for {0} minutes", tenMinuteIncrement * 10);
                lastPrint = DateTime.Now;
            }
        }
    }
}
