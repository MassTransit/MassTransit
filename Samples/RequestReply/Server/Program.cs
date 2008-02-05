using System;
using MassTransit.ServiceBus;
using MassTransit.ServiceBus.Subscriptions;
using SecurityMessages;

namespace Server
{
    using MassTransit.ServiceBus.SubscriptionsManager.Client;

    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            IMessageQueueEndpoint serverEndpoint = new MessageQueueEndpoint("msmq://localhost/test_server");
            IMessageQueueEndpoint subscriptionManagerEndpoint = new MessageQueueEndpoint("msmq://localhost/test_subscriptions");

            ISubscriptionStorage storage = new LocalSubscriptionCache();

            ServiceBus bus = new ServiceBus(serverEndpoint, storage);
            SubscriptionManagerClient subscriptionClient = new SubscriptionManagerClient(bus, storage, subscriptionManagerEndpoint);
            subscriptionClient.Start();

            bus.Subscribe<RequestPasswordUpdate>(Program_MessageReceived);

            Console.WriteLine("Thank You. Press any key to exit");
            Console.ReadKey(true);
            
        }

        static void Program_MessageReceived(IMessageContext<RequestPasswordUpdate> cxt)
        {
            Console.WriteLine("Received Message");
            Console.WriteLine(cxt.Message.NewPassword);
            cxt.Reply(new PasswordUpdateComplete(0));
        }
    }
}
