using System;
using MassTransit.ServiceBus;
using MassTransit.ServiceBus.Subscriptions;
using SecurityMessages;

namespace Client
{

    class Program
    {
        static void Main(string[] args)
        {
            IEndpoint clientEndpoint = new MessageQueueEndpoint("msmq://localhost/test_client");
            IEndpoint subscriptionMgrEndpoint = new MessageQueueEndpoint("msmq://localhost/test_subscriptionmgr");
            IEndpoint serverEndpoint = new MessageQueueEndpoint("msmq://localhost/test_server");

            ISubscriptionStorage storage = new MsmqSubscriptionStorage(new MessageQueueEndpoint("msmq://localhost/test_subscriptions"), subscriptionMgrEndpoint, new LocalSubscriptionCache());

            ServiceBus bus = new ServiceBus(clientEndpoint, storage);

            bus.Subscribe<PasswordUpdateComplete>(Program_MessageReceived);

            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();
           
            bus.Send(serverEndpoint, new RequestPasswordUpdate(newPassword));

            Console.WriteLine("Waiting For Reply");
            Console.ReadKey(true);
        }

        static void Program_MessageReceived(MessageContext<PasswordUpdateComplete> cxt)
        {
            Console.WriteLine("Password Set");
            Console.WriteLine("Thank You. Press any key to exit");
            Console.ReadKey(true);
        }
    }
}
