namespace MassTransit.ServiceBus.Configuration
{
    using System;

    public static class Via
    {
        public static SubscriptionOptions SubscriptionService(string uri)
        {
            return SubscriptionService(new Uri(uri));
        }
        public static SubscriptionOptions SubscriptionService(Uri uri)
        {
            return new SubscriptionOptions {Address = uri, SubscriptionStore =  typeof(object)};
        }

        public static SubscriptionOptions DistributedCache(string uri)
        {
            return DistributedCache(new Uri(uri));
        }
        public static SubscriptionOptions DistributedCache(Uri uri)
        {
            return new SubscriptionOptions{Address = uri};
        }


        public static SubscriptionOptions Custom<T>(string uri)
        {
            return Custom<T>(new Uri(uri));
        }
        public static SubscriptionOptions Custom<T>(Uri uri)
        {
            return new SubscriptionOptions{Address=uri, SubscriptionStore = typeof(T)};
        }
    }
}