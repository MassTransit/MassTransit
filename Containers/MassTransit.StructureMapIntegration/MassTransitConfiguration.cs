namespace MassTransit.StructureMapIntegration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Exceptions;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using ServiceBus;
    using StructureMap;

    public class MassTransitConfiguration :
        BusConfig
    {
        public MassTransitConfiguration()
        {
            StructureMapConfiguration.ForRequestedType<IObjectBuilder>().AddConcreteType<StructureMapObjectBuilder>().AsSingletons();
            StructureMapConfiguration.ForRequestedType<ISubscriptionCache>().AddConcreteType<LocalSubscriptionCache>().AsSingletons();
            StructureMapConfiguration.ForRequestedType<IEndpointResolver>().AddConcreteType<EndpointResolver>().AsSingletons();
            
        }

        //this needs to happen first
        public BusConfig WithTransport<T>()
        {
            return WithTransport(typeof (T));
        }
        public BusConfig WithTransport(Type transportType)
        {
            EndpointResolver.AddTransport(transportType);
            return this;
        }


        //then this
        BusConfig BusConfig.AddBus(string id, Uri listeningUri,
            Uri subscriptionAddress, Uri managementUri)
        {
            ISubscriptionCache cache = ResolveSubscriptionCache("name", "mode");

            //IServiceBus bus = BuildServiceBus(id, endpoint, cache, child);
            ServiceBus bus = BuildServiceBus(id, ObjectFactory.GetInstance<IEndpointResolver>().Resolve(listeningUri), cache);


            if (managementUri != null)
                SetupManagement(3, bus, id);

            if (subscriptionAddress != null)
                SetupSubscriptionClient(subscriptionAddress, bus, id, cache);

            return this;
        }



        private static ServiceBus BuildServiceBus(string id, IEndpoint endpoint, ISubscriptionCache cache)
        {
            ServiceBus bus = new ServiceBus(endpoint,
                                            ObjectFactory.GetInstance<IObjectBuilder>(),
                                            cache,
                                            ObjectFactory.GetInstance<IEndpointResolver>());

            SetupThreading(1,10, bus);

            StructureMapConfiguration.ForRequestedType<IServiceBus>()
                .AddInstance(bus); //how to set id?

            return bus;
        }
        private static void SetupThreading(int minThreads, int maxThreads, ServiceBus bus)
        {
            bus.MinThreadCount = minThreads;
            bus.MaxThreadCount = maxThreads;
        }
        private static void SetupManagement(int heartbeatInterval, IServiceBus bus, string id)
        {

            HealthClient client = new HealthClient(bus, heartbeatInterval);

            
            var i = StructureMapConfiguration.AddInstanceOf(client);
            i.Name = id + ".managementClient";
            client.Start(); //TODO: Should use startable
        }
        private static void SetupSubscriptionClient(Uri subscriptionServiceAddress, IServiceBus bus, string id, ISubscriptionCache cache)
        {

            IEndpoint subscriptionServiceEndpoint =
                ObjectFactory.GetInstance<IEndpointResolver>().Resolve(subscriptionServiceAddress);

            SubscriptionClient client = new SubscriptionClient(bus, cache, subscriptionServiceEndpoint);

            var i = StructureMapConfiguration.AddInstanceOf(client);
            i.Name = id + ".managementClient";

            client.Start(); //TODO: should use the startable
        }

        private ISubscriptionCache ResolveSubscriptionCache(string name, string mode)
        {
            //name: makes it available to others

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(mode))
                return ObjectFactory.GetInstance<ISubscriptionCache>();

            switch (mode)
            {
                case "local":
                    if (string.IsNullOrEmpty(name))
                        return ObjectFactory.GetInstance<ISubscriptionCache>();
                    else
                    {
                        ISubscriptionCache cache = ObjectFactory.GetInstance<ISubscriptionCache>(); //TODO: Name
                        if (cache == null)
                        {
                            cache = ObjectFactory.GetInstance<ISubscriptionCache>();
                            StructureMapConfiguration.ForRequestedType<ISubscriptionCache>()
                                .AddInstance(cache); //TODO: how to add name
                        }

                        return cache;
                    }

                case "distributed":
                    IList<string> servers = new List<string>(); //TODO: not sure how to best populate it yet

                    if (string.IsNullOrEmpty(name))
                    {
                        return new DistributedSubscriptionCache(GetDistributedCacheServerList(servers));
                    }
                    else
                    {
                        ISubscriptionCache cache = ObjectFactory.GetInstance<DistributedSubscriptionCache>(); //TODO: name?
                        if (cache == null)
                        {
                            cache = new DistributedSubscriptionCache(GetDistributedCacheServerList(servers));
                            StructureMapConfiguration.ForRequestedType<ISubscriptionCache>()
                                .AddInstance(cache); //TODO: How to add name
                        }

                        return cache;
                    }

                default:
                    throw new ConventionException(mode + " is not a valid subscriptionCache mode");
            }
        }

        private static IEnumerable<string> GetDistributedCacheServerList(IList<string> servers)
        {
            IList<string> result = new List<string>();

            if (servers != null)
            {
                foreach (string server in servers)
                {
                    servers.Add("memcached://" + server);
                }
            }

            return result;
        }
    }

    public interface BusConfig
    {
        BusConfig WithTransport<T>();
        BusConfig WithTransport(Type transportType);

        BusConfig AddBus(string id, Uri listeningUri,
            Uri subscriptionAddress, Uri managementUri);
    }
}
