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
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Castle.MicroKernel.Facilities;
    using Castle.MicroKernel.Registration;
    using Configuration;
    using Internal;
    using Saga;
    using Services.Subscriptions;
    using Services.Subscriptions.Configuration;
    using Services.Subscriptions.Server;
    using Transports;

    /// <summary>
    /// Facility to simplify the use of MT
    /// </summary>
    public abstract class MassTransitFacilityBase :
        AbstractFacility
    {
        /// <summary>
        /// Default constructor with not actual registration
        /// </summary>
        public MassTransitFacilityBase()
            : this(x => { })
        {
        }

        public MassTransitFacilityBase(Action<IEndpointFactoryConfigurator> configurationAction)
        {
            RegisterBusDependencies();



            //type scanning
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var files = from f in Directory.GetFiles(assemblyPath)
            where f.StartsWith("MassTransit.Transports.")
            select f;

            var types = new List<Type>();
            foreach (var file in files)
            {
                Kernel.Register( AllTypes.Of<IEndpoint>()
                    .FromAssemblyNamed(file).Configure(c=> types.Add(c.Implementation)));
            }

            RegisterEndpointFactory(x =>
            {
                types.Each(x.RegisterTransport);
                configurationAction(x);
            });
        }

        /// <summary>
        /// Creates a registry for a service bus listening to an endpoint
        /// </summary>
        public MassTransitFacilityBase(params Type[] transportTypes)
        {
            RegisterBusDependencies();

            RegisterEndpointFactory(x =>
            {
                x.RegisterTransport<LoopbackEndpoint>();
                x.RegisterTransport<MulticastUdpEndpoint>();

                foreach (Type type in transportTypes)
                {
                    x.RegisterTransport(type);
                }
            });
        }


        /// <summary>
        /// Registers the in-memory subscription service so that all buses created in the same
        /// process share subscriptions
        /// </summary>
        protected void RegisterInMemorySubscriptionService()
        {
            Kernel.Register(
                Component.For<IEndpointSubscriptionEvent>().ImplementedBy<LocalSubscriptionService>().LifeStyle.Singleton,
                Component.For<SubscriptionPublisher>().ImplementedBy<SubscriptionPublisher>(),
                Component.For<SubscriptionConsumer>().ImplementedBy<SubscriptionConsumer>()
                );
        }

        protected void RegisterInMemorySubscriptionRepository()
        {
            Kernel.Register(
                Component.For<ISubscriptionRepository>().ImplementedBy<InMemorySubscriptionRepository>().LifeStyle.Singleton
                );
        }

        protected void RegisterInMemorySagaRepository()
        {
            Kernel.Register(
                Component.For(typeof(ISagaRepository<>)).ImplementedBy(typeof(InMemorySagaRepository<>)).LifeStyle.Singleton
                );
        }

        /// <summary>
        /// Registers the types used by the service bus internally and as part of the container.
        /// These are typically items that are not swapped based on the container implementation
        /// </summary>
        protected void RegisterBusDependencies()
        {
            Kernel.Register(
                Component.For<IObjectBuilder>().ImplementedBy<WindsorObjectBuilder>().LifeStyle.Singleton
                );

            //we are expecting SM to auto-resolve
            // SubscriptionClient
            // InitiateSagaMessageSink<,>
            // OrchestrateSagaMessageSink<,>)
            // InitiateSagaStateMachineSink<,>)
            // OrchestrateSagaStateMachineSink<,>)
        }

        protected void RegisterEndpointFactory(Action<IEndpointFactoryConfigurator> configAction)
        {
            Kernel.Register(
                Component.For<IEndpointFactory>().UsingFactoryMethod((kernel, context) =>
                {
                    return EndpointFactoryConfigurator.New(x =>
                    {
                        x.SetObjectBuilder(kernel.Resolve<IObjectBuilder>());
                        configAction(x);
                    });
                }).LifeStyle.Singleton
                );
        }

        protected void RegisterServiceBus(string endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            RegisterServiceBus(new Uri(endpointUri), configAction);
        }

        protected void RegisterServiceBus(Uri endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            Kernel.Register(
                Component.For<IServiceBus>().UsingFactoryMethod((kernel, context) =>
                {
                    return ServiceBusConfigurator.New(x =>
                    {
                        x.SetObjectBuilder(kernel.Resolve<IObjectBuilder>());
                        x.ReceiveFrom(endpointUri);

                        configAction(x);
                    });
                }).LifeStyle.Singleton);
        }

        protected void RegisterControlBus(string endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            RegisterControlBus(new Uri(endpointUri), configAction);
        }

        protected void RegisterControlBus(Uri endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            Kernel.Register(
                Component.For<IControlBus>().UsingFactoryMethod((kernel, context) =>
                {
                    return ControlBusConfigurator.New(x =>
                    {
                        x.SetObjectBuilder(kernel.Resolve<IObjectBuilder>());
                        x.ReceiveFrom(endpointUri);
                        x.SetConcurrentConsumerLimit(1);

                        configAction(x);
                    });
                }).LifeStyle.Singleton
                );
        }

        protected static void ConfigureSubscriptionClient(string subscriptionServiceEndpointAddress, IServiceBusConfigurator configurator)
        {
            ConfigureSubscriptionClient(new Uri(subscriptionServiceEndpointAddress), configurator);
        }

        protected static void ConfigureSubscriptionClient(Uri subscriptionServiceEndpointAddress, IServiceBusConfigurator configurator)
        {
            configurator.ConfigureService<SubscriptionClientConfigurator>(y =>
            {
                // this is fairly easy inline, but wanted to include the example for completeness
                y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
            });
        }
    }
}