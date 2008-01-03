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
            IEndpoint subscriptionMgrEndpoint = MessageQueueEndpoint.MessageQueueEndpointFactory.Instance.Resolve(@".\private$\test_subscriptionMgr");
            IEndpoint serverEndpoint = MessageQueueEndpoint.MessageQueueEndpointFactory.Instance.Resolve(@".\private$\test_server");

            ISubscriptionStorage storage = new MsmqSubscriptionStorage(@".\private$\test_subscriptions", subscriptionMgrEndpoint, new SubscriptionCache());

            ServiceBus bus = new ServiceBus(serverEndpoint, storage);
            
            bus.MessageEndpoint<RequestPasswordUpdate>().Subscribe(Program_MessageReceived);

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
