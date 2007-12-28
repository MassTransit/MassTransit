namespace Server
{
    using System;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using SecurityMessages;

    class Program
    {
        static void Main(string[] args)
        {
            IEndpoint endpointa = new MessageQueueEndpoint(@".\private$\endpointc");
            ServiceBus bus = new ServiceBus(endpointa);
            bus.SubscriptionStorage = new MsmqSubscriptionStorage(@".\private$\test_subscriptions", new MessageQueueEndpoint(@".\private$\test"), new SubscriptionCache());

            bus.Subscribe<RequestPasswordUpdate>().MessageReceived += Program_MessageReceived;
            Console.WriteLine("Thank You. Press any key to exit");
            Console.ReadKey(true);
            
        }

        static void Program_MessageReceived(IServiceBus bus, IEnvelope envelope, RequestPasswordUpdate message)
        {
            Console.WriteLine("Received Message");
            Console.WriteLine(message.NewPassword);
            bus.Send(envelope.ReturnTo, new PasswordUpdateComplete(0));
        }
    }
}
