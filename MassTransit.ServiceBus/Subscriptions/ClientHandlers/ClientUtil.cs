namespace MassTransit.ServiceBus.Subscriptions.ClientHandlers
{
    public class ClientUtil
    {
        public static bool IsOwnedSubscription(Subscription subscription, IServiceBus bus)
        {
            if (subscription.EndpointUri == bus.Endpoint.Uri)
                return true;

            return false;
        }
    }
}