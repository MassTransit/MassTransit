using System;
using MassTransit.ServiceBus;
using MassTransit.ServiceBus.Subscriptions;
using SecurityMessages;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            IEndpoint subscriptionMgrEndpoint = new MessageQueueEndpoint("msmq://localhost/test_subscriptionmgr");
            IEndpoint serverEndpoint = new MessageQueueEndpoint("msmq://localhost/test_server");

            ISubscriptionStorage storage = new MsmqSubscriptionStorage(new MessageQueueEndpoint("msmq://localhost/test_subscriptions"), subscriptionMgrEndpoint, new LocalSubscriptionCache());

            ServiceBus bus = new ServiceBus(serverEndpoint, storage);
            
            bus.Subscribe<RequestPasswordUpdate>(Program_MessageReceived);

            Console.WriteLine("Thank You. Press any key to exit");
            Console.ReadKey(true);
            
        }

        static void Program_MessageReceived(MessageContext<RequestPasswordUpdate> cxt)
        {
            Console.WriteLine("Received Message");
            Console.WriteLine(cxt.Message.NewPassword);
            cxt.Reply(new PasswordUpdateComplete(0));
        }
    }
}
