namespace MassTransit.ServiceBus.Configuration
{
    using System;
    using Internal;

    public class BusOptions
    {
        public BusOptions()
        {
            ListensOn = EndpointResolver.Null.Uri;
            CommandedOn = EndpointResolver.Null.Uri;
            Serialization = Serializers.Binary;
        }
        public Uri ListensOn { get; set; }
        public Uri CommandedOn { get; set; }
        public bool IsACompetingConsumer { get; private set; }
        public void MarkAsCompetingConsumer()
        {
            IsACompetingConsumer = true;
        }
        public SerializationOptions Serialization { get; set; }
        public SubscriptionOptions Subcriptions { get; set; }
        public void RegisterTransport<T>()
        {
            EndpointResolver.AddTransport(typeof(T));
        }
        public string Name { get; set; }
        public object ResolvesDependenciesFrom { get; set; }
    }
}