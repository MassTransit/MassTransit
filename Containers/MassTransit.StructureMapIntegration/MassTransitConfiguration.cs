namespace MassTransit.StructureMapIntegration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Exceptions;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using ServiceBus;
    using ServiceBus.Services.HealthMonitoring;
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

            IEndpoint busEndpoint = ObjectFactory.GetInstance<IEndpointResolver>().Resolve(listeningUri);
            ServiceBus bus = BuildServiceBus(id, busEndpoint, cache);


            if (managementUri != null)
                SetupManagement(id, bus, 3);

            if (subscriptionAddress != null)
                SetupSubscriptionClient(id, bus, subscriptionAddress, cache);

            return this;
        }



        private static ServiceBus BuildServiceBus(string id, IEndpoint endpoint, ISubscriptionCache cache)
        {
            ServiceBus bus = new ServiceBus(endpoint,
                                            ObjectFactory.GetInstance<IObjectBuilder>(),
                                            cache,
                                            ObjectFactory.GetInstance<IEndpointResolver>());

            SetupThreading(1,10, bus);

            StructureMapConfiguration.AddInstanceOf<IServiceBus>(bus).WithName(id);

            return bus;
        }
        private static void SetupThreading(int minThreads, int maxThreads, ServiceBus bus)
        {
            bus.MinThreadCount = minThreads;
            bus.MaxThreadCount = maxThreads;
        }
        private static void SetupManagement(string id, IServiceBus bus, int heartbeatInterval)
        {
            HealthClient client = new HealthClient(bus, heartbeatInterval);
            StructureMapConfiguration.AddInstanceOf<HealthClient>(client).WithName(id + ".managementClient");
            client.Start(); //TODO: Should use startable
        }
        private static void SetupSubscriptionClient(string id, IServiceBus bus, Uri subscriptionServiceAddress, ISubscriptionCache cache)
        {

            IEndpoint subscriptionServiceEndpoint =
                ObjectFactory.GetInstance<IEndpointResolver>().Resolve(subscriptionServiceAddress);

            SubscriptionClient client = new SubscriptionClient(bus, cache, subscriptionServiceEndpoint);

            var i = StructureMapConfiguration.AddInstanceOf(client);
            i.Name = id + ".managementClient";

            client.Start(); //TODO: should use the startable
        }

        private static ISubscriptionCache ResolveSubscriptionCache(string name, string mode)
        {
            ISubscriptionCache cache = null;

            //name: makes it available to others
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(mode))
                return ObjectFactory.GetNamedInstance<ISubscriptionCache>(name);

            switch (mode)
            {
                case "local":
                    if (string.IsNullOrEmpty(name))
                        return ObjectFactory.GetInstance<ISubscriptionCache>();

                    cache = ObjectFactory.GetNamedInstance<ISubscriptionCache>(name);
                    if (cache == null)
                    {
                        cache = ObjectFactory.GetInstance<ISubscriptionCache>();
                        StructureMapConfiguration.AddInstanceOf(cache).WithName(name);
                    }

                    return cache;


                case "distributed":
                    IList<string> servers = new List<string>(); //TODO: not sure how to best populate it yet

                    if (string.IsNullOrEmpty(name))
                        return new DistributedSubscriptionCache(GetDistributedCacheServerList(servers));

                    cache = ObjectFactory.GetNamedInstance<DistributedSubscriptionCache>(name);
                    //TODO: name?
                    if (cache == null)
                    {
                        cache = new DistributedSubscriptionCache(GetDistributedCacheServerList(servers));
                        StructureMapConfiguration.AddInstanceOf(cache).WithName(name);
                    }

                    return cache;


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
