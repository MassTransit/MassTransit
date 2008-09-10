namespace MassTransit.ServiceBus.Configuration
{
    using System;
    using Formatters;
    using Internal;
    using Subscriptions;

    public class BusOptions
    {
        public IObjectBuilder ResolvesDependenciesFrom { get; private set; }



        public BusOptions(IObjectBuilder objectBuilder)
        {
            ResolvesDependenciesFrom = objectBuilder;
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
        public bool HasAHeartBeat { get; private set; }
        public void TurnOnHeartBeat()
        {
            HasAHeartBeat = true;
        }

        public IServiceBus Build()
        {
            IEndpoint endpointToListenOn = ResolvesDependenciesFrom.Build<IEndpointResolver>().Resolve(ListensOn);
            //commanded on
            //competing consumer
            //serialization stuff
            ISubscriptionCache cache = ResolvesDependenciesFrom.Build<ISubscriptionCache>(this.Subcriptions.SubscriptionStore);
            //ResolvesDependenciesFrom.Register(this.Subcriptions.SubscriptionClient); //set the uri
            IServiceBus bus = new ServiceBus(endpointToListenOn, ResolvesDependenciesFrom, cache);
            //ResolvesDependenciesFrom.Register<IServiceBus>(bus, Name);
            
            //has a heart beat

            return bus;
        }


    }
}