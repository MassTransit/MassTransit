namespace Client
{
    using System;
    using System.Transactions;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using SecurityMessages;

    class Program
    {
        static void Main(string[] args)
        {
            IEndpoint clientEndpoint = MessageQueueEndpoint.MessageQueueEndpointFactory.Instance.Resolve(@".\private$\test_client");
            IEndpoint subscriptionMgrEndpoint = MessageQueueEndpoint.MessageQueueEndpointFactory.Instance.Resolve(@".\private$\test_subscriptionMgr");
            IEndpoint serverEndpoint = MessageQueueEndpoint.MessageQueueEndpointFactory.Instance.Resolve(@".\private$\test_server");

            ISubscriptionStorage storage = new MsmqSubscriptionStorage(@".\private$\test_subscriptions", subscriptionMgrEndpoint, new SubscriptionCache());
            ServiceBus bus = new ServiceBus(clientEndpoint, storage);

            bus.MessageEndpoint<PasswordUpdateComplete>().Subscribe(Program_MessageReceived);

            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            using (TransactionScope tr = new TransactionScope())
            {
                bus.Send(serverEndpoint, new RequestPasswordUpdate(newPassword));

                tr.Complete();
            }

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
