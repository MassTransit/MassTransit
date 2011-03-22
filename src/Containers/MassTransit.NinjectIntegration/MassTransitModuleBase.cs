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
namespace MassTransit.NinjectIntegration
{
    using System;
    using Configuration;
    using Internal;
    using Ninject;
    using Ninject.Modules;
    using Saga;
    using Services.Subscriptions;
    using Services.Subscriptions.Configuration;
    using Services.Subscriptions.Server;
    using Transports;

    /// <summary>
    /// This is an extension of the Ninject Module exposing methods to make it easy to get Mass
    /// Transit set up.
    /// </summary>
    public class MassTransitModuleBase :
        NinjectModule
    {
        private readonly Action<IEndpointFactoryConfigurator> _ConfigurationAction;
        private readonly Type[] _TransportTypes;
        private IObjectBuilder _Builder;

        protected IObjectBuilder Builder { get { return _Builder; } }

        public MassTransitModuleBase(IObjectBuilder builder)
            : this(builder, null, (Type[])null)
        {
        }

        public MassTransitModuleBase(IObjectBuilder builder,
            Action<IEndpointFactoryConfigurator> configurationAction)
            : this(builder, configurationAction, null)
        {
        }

        /// <summary>
        /// Creates a registry for a service bus listening to an endpoint
        /// </summary>
        public MassTransitModuleBase(IObjectBuilder builder,
            params Type[] transportTypes)
            : this(builder, null, transportTypes)
        {
        }

        /// <summary>
        /// Creates a new instance of MassTransitModuleBase.
        /// </summary>
        /// <remarks>
        /// With NInject, its not possible to actually build anything yet,
        /// because the underlying binding mechanism does not exist at
        /// construction time. We will save the values and do the setup later
        /// when Load() is called.
        /// </remarks>
        /// <param name="builder">
        /// </param>
        /// <param name="configurationAction">
        /// The endpoint factory configuration action to use when creating an
        /// endpoint factory.
        /// </param>
        /// <param name="transportTypes">
        /// The transport types to configure.
        /// </param>
        public MassTransitModuleBase(IObjectBuilder builder,
            Action<IEndpointFactoryConfigurator> configurationAction,
            params Type[] transportTypes)
        {
            _Builder = builder;

            if (configurationAction == null)
            {
                _ConfigurationAction = DefaultEndpointFactoryConfigurator;
            }
            else
            {
                _ConfigurationAction = configurationAction;
            }

            _TransportTypes = transportTypes;
        }

        public override void Load()
        {
            RegisterBusDependencies();

            if (_TransportTypes != null)
            {
                RegisterEndpointFactory(x => _ConfigurationAction(x));
            }
        }

        protected void DefaultEndpointFactoryConfigurator(IEndpointFactoryConfigurator endpointFactoryConfigurator)
        {
            endpointFactoryConfigurator.RegisterTransport<LoopbackEndpoint>();
            endpointFactoryConfigurator.RegisterTransport<MulticastUdpEndpoint>();

            foreach (Type type in _TransportTypes)
            {
                endpointFactoryConfigurator.RegisterTransport(type);
            }
        }

        /// <summary>
        /// Registers the in-memory subscription service so that all buses created in the same
        /// process share subscriptions
        /// </summary>
        protected void RegisterInMemorySubscriptionService()
        {
            Bind<IEndpointSubscriptionEvent>()
                .To<LocalSubscriptionService>()
                .InSingletonScope();

            Bind<SubscriptionPublisher>()
                .To<SubscriptionPublisher>();

            Bind<SubscriptionConsumer>()
                .To<SubscriptionConsumer>();
        }

        protected void RegisterInMemorySubscriptionRepository()
        {
            Bind<ISubscriptionRepository>()
                .To<InMemorySubscriptionRepository>()
                .InSingletonScope();
        }

        protected void RegisterInMemorySagaRepository()
        {
            Bind(typeof(ISagaRepository<>))
                .To(typeof(InMemorySagaRepository<>))
                .InSingletonScope();
        }

        /// <summary>
        /// Registers the types used by the service bus internally and as part of the container.
        /// These are typically items that are not swapped based on the container implementation
        /// </summary>
        protected void RegisterBusDependencies()
        {
            //how singlton
            Bind<IObjectBuilder>().To<NinjectObjectBuilder>().InSingletonScope();


            //we are expecting NINJECT to auto-resolve
            // SubscriptionClient
            // InitiateSagaMessageSink<,>
            // OrchestrateSagaMessageSink<,>)
            // InitiateSagaStateMachineSink<,>)
            // OrchestrateSagaStateMachineSink<,>)
        }

        protected void RegisterEndpointFactory(Action<IEndpointFactoryConfigurator> configAction)
        {
            Bind<IEndpointFactory>()
                .ToMethod(cxt =>
                {
                    return EndpointFactoryConfigurator.New(x =>
                    {
                        x.SetObjectBuilder(cxt.Kernel.Get<IObjectBuilder>());
                        configAction(x);
                    });
                }).InSingletonScope();
        }

        protected void RegisterServiceBus(string endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            RegisterServiceBus(new Uri(endpointUri), configAction);
        }

        protected void RegisterServiceBus(Uri endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            Bind<IServiceBus>()
                .ToMethod(context =>
                {
                    return ServiceBusConfigurator.New(x =>
                    {
                        x.SetObjectBuilder(context.Kernel.Get<IObjectBuilder>());
                        x.ReceiveFrom(endpointUri);

                        configAction(x);
                    });
                })
                .InSingletonScope();
        }

        protected void RegisterControlBus(string endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            RegisterControlBus(new Uri(endpointUri), configAction);
        }

        protected void RegisterControlBus(Uri endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            Bind<IControlBus>()
                .ToMethod(context =>
                {
                    return ControlBusConfigurator.New(x =>
                    {
                        x.SetObjectBuilder(context.Kernel.Get<IObjectBuilder>());
                        x.ReceiveFrom(endpointUri);
                        x.SetConcurrentConsumerLimit(1);

                        configAction(x);
                    });
                }).InSingletonScope();
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