// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.WindsorIntegration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Castle.Core;
    using Castle.Core.Configuration;
    using Castle.Facilities.Startable;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Facilities;
    using Castle.MicroKernel.Registration;
    using Infrastructure.Subscriptions;
    using MassTransit.Exceptions;
    using MassTransit.Internal;
    using MassTransit.Subscriptions;
    using Microsoft.Practices.ServiceLocation;
	using MassTransit.Pipeline.Sinks;
    using Saga.Pipeline;
    using Services.HealthMonitoring;
    using Component = Castle.MicroKernel.Registration.Component;

    /// <summary>
    /// Facility to simplify the use of MT
    /// </summary>
    public class MassTransitFacility :
        AbstractFacility
    {
        protected override void Init()
        {
            //Kernel.Resolver.AddSubResolver(new ServiceLocatorResolver());
            //SetupAutoRegister();

            LoadTransports();
            RegisterComponents();

            LoadServiceBuses();

            //AddStartableFacility();
        }

        public void AddStartableFacility()
        {
            foreach (IFacility facility in Kernel.GetFacilities())
            {
                if (facility.GetType().Equals(typeof (StartableFacility)))
                {
                    return;
                }
            }
        }
        private void SetupAutoRegister()
        {
            if (Convert.ToBoolean(FacilityConfig.Attributes["auto-register"]))
            {
                Kernel.ComponentRegistered += OnComponentRegistered;
            }
        }
        private void OnComponentRegistered(string key, IHandler handler)
        {
            //TODO: Could throw
            var bus = this.Kernel.Resolve<IServiceBus>();
            Type consumerType = typeof (IConsumer);
            if(consumerType.IsAssignableFrom(handler.ComponentModel.Implementation))
            {
				// TODO this is broken and needs to use something better to invoke the generic method on IServiceBus

                bus.Subscribe(handler.ComponentModel.Implementation);
            }
        }

        private void LoadTransports()
        {
            var transportConfiguration = FacilityConfig.Children["transports"];
            if (transportConfiguration == null)
                throw new ConventionException("At least one transport must be defined in the facility configuration.");

            foreach (IConfiguration transport in transportConfiguration.Children)
            {
                Type transportType = Type.GetType(transport.Value, true, true);

                string scheme = EndpointResolver.AddTransport(transportType);

                Kernel.Register(
                    Component.For(transportType)
                        .ImplementedBy(transportType)
                        .AddAttributeDescriptor("factoryId", "endpoint.factory")
                        .AddAttributeDescriptor("factoryCreate", "Resolve")
                        .LifeStyle.Transient
                        .Named("transport." + scheme)
                    );
            }
        }

        private void RegisterComponents()
        {
            Kernel.AddComponentInstance("kernel", typeof(IKernel), Kernel);

            Kernel.Register(
                Component.For<ISubscriptionCache>()
                    .ImplementedBy<LocalSubscriptionCache>()
                    .LifeStyle.Singleton,
                Component.For<IEndpointResolver>()
                    .ImplementedBy<EndpointResolver>()
                    .Named("endpoint.factory")
                    .LifeStyle.Singleton,
                Component.For<ITypeInfoCache>()
                    .ImplementedBy<TypeInfoCache>()
                    .Named("typeinfocache")
                    .LifeStyle.Singleton
                );

        	Kernel.AddComponent("initiateSagaMessageSink", typeof (InitiateSagaMessageSink<,>), LifestyleType.Transient);
        	Kernel.AddComponent("orchestrateSagaMessageSink", typeof (OrchestrateSagaMessageSink<,>), LifestyleType.Transient);
        }

        private void LoadServiceBuses()
        {
            foreach (IConfiguration child in FacilityConfig.Children)
            {
                if (child.Name.Equals("bus"))
                {
                    string id = child.Attributes["id"];
                    string endpointUri = child.Attributes["endpoint"];

                    IEndpoint endpoint = Kernel.Resolve<IEndpointResolver>().Resolve(new Uri(endpointUri));

                    ISubscriptionCache cache = ResolveSubscriptionCache(child);

                    IServiceBus bus = BuildServiceBus(id, endpoint, cache, child);

                    ResolveSubscriptionClient(child, bus, id, cache);
                    ResolveManagementClient(child, bus, id);
                }
            }
        }

        private IServiceBus BuildServiceBus(string id, IEndpoint endpoint, ISubscriptionCache cache, IConfiguration busConfig)
        {
            ServiceBus bus = new ServiceBus(endpoint,
                                            Kernel.Resolve<IObjectBuilder>(),
                                            cache,
                                            Kernel.Resolve<IEndpointResolver>(),
                                            Kernel.Resolve<ITypeInfoCache>());

            IConfiguration threadConfig = busConfig.Children["dispatcher"];
            ConfigureThreadingModel(threadConfig, bus);

            Kernel.AddComponentInstance(id, typeof(IServiceBus), bus);

            return bus;
        }

        private void ConfigureThreadingModel(IConfiguration threadConfig, ServiceBus bus)
        {
            if (threadConfig != null)
            {
                bus.MinThreadCount = GetConfigurationValue(threadConfig, "minThreads", bus.MinThreadCount);
                bus.MaxThreadCount = GetConfigurationValue(threadConfig, "maxThreads", bus.MaxThreadCount);
                bus.ReadThreadCount = GetConfigurationValue(threadConfig, "readThreads", bus.ReadThreadCount);
            }
        }
        private void ResolveManagementClient(IConfiguration child, IServiceBus bus, string id)
        {
            var managementClientConfig = child.Children["managementService"];
            if (managementClientConfig != null)
            {
                string heartbeatInterval = managementClientConfig.Attributes["heartbeatInterval"];

                int interval = string.IsNullOrEmpty(heartbeatInterval) ? 3 : int.Parse(heartbeatInterval);

                HealthClient sc = new HealthClient(bus, interval);
                bus.Subscribe(sc);

                Kernel.AddComponentInstance(id + ".managementClient", sc);
                sc.Start(); //TODO: Should use startable
            }
        }
        private void ResolveSubscriptionClient(IConfiguration child, IServiceBus bus, string id, ISubscriptionCache cache)
        {
            var subscriptionClientConfig = child.Children["subscriptionService"];
            if (subscriptionClientConfig != null)
            {
                string subscriptionServiceEndpointUri = subscriptionClientConfig.Attributes["endpoint"];

                IEndpoint subscriptionServiceEndpoint =
                    Kernel.Resolve<IEndpointResolver>().Resolve(new Uri(subscriptionServiceEndpointUri));

                SubscriptionClient sc = new SubscriptionClient(bus, cache, subscriptionServiceEndpoint);

            	IConfiguration localEndpointConfig = subscriptionClientConfig.Children["localEndpoint"];
				if (localEndpointConfig != null)
				{
					IEndpoint localEndpoint =
						Kernel.Resolve<IEndpointResolver>().Resolve(new Uri(localEndpointConfig.Value));

					sc.AddLocalEndpoint(localEndpoint);
				}

                Kernel.AddComponentInstance(id + ".subscriptionClient", sc);
                sc.Start(); //TODO: should use the startable
            }
        }


        private ISubscriptionCache ResolveSubscriptionCache(IConfiguration configuration)
        {
            var cacheConfig = configuration.Children["subscriptionCache"];
            if (cacheConfig == null)
                return Kernel.Resolve<ISubscriptionCache>();

            // naming the cache makes it available to others
            string name = cacheConfig.Attributes["name"];

        	string mode = cacheConfig.Attributes["mode"] ?? "local";
            ISubscriptionCache cache;
            switch (mode)
            {
                case "local":
                    if (string.IsNullOrEmpty(name))
                        return Kernel.Resolve<ISubscriptionCache>();
                    
                    cache = Kernel.Resolve<ISubscriptionCache>(name);
                    if (cache == null)
                    {
                        cache = Kernel.Resolve<ISubscriptionCache>();
                        Kernel.AddComponentInstance(name, cache);
                    }

                    return cache;

                case "distributed":
                    if (string.IsNullOrEmpty(name))
                        return new DistributedSubscriptionCache(GetDistributedCacheServerList(cacheConfig));
                    
                    cache = Kernel.Resolve<DistributedSubscriptionCache>(name);
                    if (cache == null)
                    {
                        cache = new DistributedSubscriptionCache(GetDistributedCacheServerList(cacheConfig));
                        Kernel.AddComponentInstance(name, cache);
                    }
                    return cache;

                default:
                    throw new ConventionException(mode + " is not a valid subscriptionCache mode");
            }
        }

        private static IEnumerable<string> GetDistributedCacheServerList(IConfiguration configuration)
        {
            var servers = new List<string>();

            var serversConfig = configuration.Children["servers"];
            if (serversConfig != null) //TODO: If this is null shouldn't we throw?
            {
                foreach (IConfiguration serverConfig in serversConfig.Children)
                {
                    servers.Add("memcached://" + serverConfig.Value);
                }
            }

            return servers;
        }


        private static ComponentRegistration<T> StartableComponent<T>()
        {
            return Component.For<T>()
                .AddAttributeDescriptor("startable", "true")
                .AddAttributeDescriptor("startMethod", "Start")
                .AddAttributeDescriptor("stopMethod", "Stop")
                .LifeStyle.Transient;
        }

        private static T GetConfigurationValue<T>(IConfiguration config, string attributeName, T defaultValue)
        {
            string value = config.Attributes[attributeName];
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));

            T newValue = (T)tc.ConvertFromInvariantString(value);

            return newValue;
        }

    }

    internal class ServiceLocatorResolver 
        : ISubDependencyResolver
    {
        private static Type serviceLocatorType = typeof (IServiceLocator);

        public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
        {
            return ServiceLocator.Current;
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
        {
            return serviceLocatorType.IsAssignableFrom(model.Service);
        }
    }
}
