using MassTransit.ServiceBus.MSMQ;

namespace Client
{
    using System;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using SecurityMessages;

    internal class Program
    {
        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            IEndpoint clientEndpoint = new MsmqEndpoint("msmq://localhost/test_client");
            IEndpoint subscriptionManagerEndpoint = new MsmqEndpoint("msmq://localhost/test_subscriptions");


            ServiceBus bus = new ServiceBus(clientEndpoint);

            SubscriptionClient subscriptionClient = new SubscriptionClient(bus, bus.SubscriptionCache, subscriptionManagerEndpoint);
            subscriptionClient.Start();

            bus.Subscribe<PasswordUpdateComplete>(Program_MessageReceived);

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            Console.WriteLine(new string('-', 20));
            bus.Publish(new RequestPasswordUpdate(newPassword));

            Console.WriteLine("Waiting For Reply");
            Console.WriteLine(new string('-', 20));
            Console.ReadKey();

            bus.Dispose();
        }

        private static void Program_MessageReceived(IMessageContext<PasswordUpdateComplete> cxt)
        {
            Console.WriteLine("Password Set!");
            Console.WriteLine("Thank You. Press any key to exit");
        }
    }
}