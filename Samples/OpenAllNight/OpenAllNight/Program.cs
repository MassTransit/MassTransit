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

            IServiceBus bus = c.Resolve<IServiceBus>();
            bus.AddComponent<CacheUpdateResponseHandler>();


            IEndpoint ep = c.Resolve<IEndpointResolver>().Resolve(new Uri("msmq://localhost/mt_pubsub"));
            Counter counter = c.Resolve<Counter>();
            Console.WriteLine("Please enter the number of hours you would like this test to run for?");
            string input = Console.ReadLine();
            double hours = double.Parse(input);
            DateTime stopTime = DateTime.Now.AddHours(hours);

            Console.WriteLine("Test Started");

            while (DateTime.Now < stopTime )
            {
                ep.Send(new CacheUpdateRequest(new Uri("msmq://localhost/test_servicebus")));
                counter.IncrementMessagesSent();
                Thread.Sleep(20);
                PrintTime();
            }

            Console.WriteLine("Messages Sent: {0}", counter.MessagesSent);
            Console.WriteLine("Messages Received: {0}", counter.MessagesReceived);
            Console.WriteLine("Done");
            Console.ReadLine();
            
        }

        private static DateTime lastPrint = DateTime.Now;
        private static int tenMinuteIncrement = 0;

        private static void PrintTime()
        {
            TimeSpan ts = DateTime.Now.Subtract(lastPrint);
            if(ts.Minutes >= 5)
            {
                tenMinuteIncrement++;
                Console.WriteLine("Test has been running for {0} minutes", tenMinuteIncrement * 5);
                lastPrint = DateTime.Now;
            }
        }
    }
}
