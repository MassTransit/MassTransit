namespace MassTransit.ServiceBus.Configuration
{
    using System;
    using Formatters;
    using Internal;
    using Subscriptions;

    public class BusOptions
    {
        private readonly IObjectBuilder _objectBuilder;

        public BusOptions(IObjectBuilder objectBuilder)
        {
            _objectBuilder = objectBuilder;
            ListensOn = EndpointResolver.Null.Uri;
            CommandedOn = EndpointResolver.Null.Uri;
            Serialization = new SerializationOptions {Serializer =  typeof(BinaryBodyFormatter)};
            Subcriptions = new SubscriptionOptions();
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
        public bool HasAHeartBeat { get; private set; }
        public void TurnOnHeartBeat()
        {
            HasAHeartBeat = true;
        }

        public IServiceBus Build()
        {
            IEndpoint ep = _objectBuilder.Build<IEndpointResolver>().Resolve(ListensOn);
            ISubscriptionCache cache = _objectBuilder.Build<ISubscriptionCache>(this.Subcriptions.SubscriptionStore);

            return new ServiceBus(ep, _objectBuilder, cache);
        }


    }
}