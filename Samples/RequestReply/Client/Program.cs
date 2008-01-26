using System;
using MassTransit.ServiceBus;
using MassTransit.ServiceBus.Subscriptions;
using SecurityMessages;

namespace Client
{
    using MassTransit.ServiceBus.SubscriptionsManager.Client;

    class Program
    {
        static void Main(string[] args)
        {
            IEndpoint clientEndpoint = new MessageQueueEndpoint("msmq://localhost/test_client");
            IEndpoint wellKnown = new MessageQueueEndpoint("msmq://localhost/test_subscriptions");

            ISubscriptionStorage storage = new LocalSubscriptionCache();

            ServiceBus bus = new ServiceBus(clientEndpoint, storage);
            ClientProxy proxy = new ClientProxy(wellKnown);
            proxy.StartWatching(bus, bus.SubscriptionStorage);

            bus.Subscribe<PasswordUpdateComplete>(Program_MessageReceived);

            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();
           
            bus.Publish(new RequestPasswordUpdate(newPassword));

            Console.WriteLine("Waiting For Reply");
            Console.ReadKey(true);
        }

        static void Program_MessageReceived(IMessageContext<PasswordUpdateComplete> cxt)
        {
            Console.WriteLine("Password Set");
            Console.WriteLine("Thank You. Press any key to exit");
            Console.ReadKey(true);
        }
    }
}
