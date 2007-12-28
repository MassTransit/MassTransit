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
            IEndpoint endpointa = new MessageQueueEndpoint(@".\private$\endpointd");
            ServiceBus bus = new ServiceBus(endpointa);
            bus.SubscriptionStorage = new MsmqSubscriptionStorage(@".\private$\test_subscriptions", new MessageQueueEndpoint(@".\private$\test"), new SubscriptionCache());

            bus.Subscribe<PasswordUpdateComplete>().MessageReceived += Program_MessageReceived;

            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            using (TransactionScope tr = new TransactionScope())
            {
                IEndpoint endpointb = new MessageQueueEndpoint(@".\private$\endpointc");
                bus.Send(endpointb, new RequestPasswordUpdate(newPassword));

                tr.Complete();
            }

            Console.WriteLine("Waiting For Reply");
            Console.ReadKey(true);
        }

        static void Program_MessageReceived(IServiceBus bus, IEnvelope envelope, PasswordUpdateComplete message)
        {
            Console.WriteLine("Password Set");
            Console.WriteLine("Thank You. Press any key to exit");
            Console.ReadKey(true);
        }
    }
}
