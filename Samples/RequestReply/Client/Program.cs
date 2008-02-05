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

            IMessageQueueEndpoint clientEndpoint = new MessageQueueEndpoint("msmq://localhost/test_client");
            IMessageQueueEndpoint subscriptionManagerEndpoint = new MessageQueueEndpoint("msmq://localhost/test_subscriptions");

            ISubscriptionStorage storage = new LocalSubscriptionCache();

            ServiceBus bus = new ServiceBus(clientEndpoint, storage);
            SubscriptionManagerClient subscriptionClient = new SubscriptionManagerClient(bus, storage, subscriptionManagerEndpoint);

            subscriptionClient.Start();

            bus.Subscribe<PasswordUpdateComplete>(Program_MessageReceived);

            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            bus.Publish(new RequestPasswordUpdate(newPassword));

            Console.WriteLine("Waiting For Reply");
            Console.ReadKey();

            bus.Dispose();
        }

        private static void Program_MessageReceived(IMessageContext<PasswordUpdateComplete> cxt)
        {
            Console.WriteLine("Password Set");
            Console.WriteLine("Thank You. Press any key to exit");
        }
    }
}