namespace MassTransit.NinjectIntegration
{
    using System;
    using Ninject;
    using Ninject.Modules;
    using Configuration;
    using Services.HealthMonitoring.Configuration;
    using Services.Subscriptions.Configuration;

    /// <summary>
    /// This is an extension of the Ninject Module exposing methods to make it easy to get Mass
    /// Transit set up.
    /// </summary>
    public class MassTransitModule :
        Module
    {
		public virtual int ConcurrentConsumerLimit { get { return 10; } }
		public virtual int ConcurrentReceiverLimit { get { return 1; } }
	
        public override void Load()
        {
            Bind<IObjectBuilder>()
                .To<NinjectObjectBuilder>();

            Bind<IServiceBus>()
                .To<ServiceBus>();

            ServiceBusConfigurator.Defaults(x => x.SetObjectBuilder(Kernel.Get<IObjectBuilder>()));
        }

        public void AddTransport<TTransport>() where TTransport : IEndpoint
        {
            AddTransports(typeof(TTransport));
        }

        public void AddTransports(params Type[] transportTypes)
        {
            RegisterEndpointFactory(x =>
            {
                foreach (var transport in transportTypes)
                {
                    x.RegisterTransport(transport);
                }
            });
        }

        private void RegisterEndpointFactory(Action<IEndpointFactoryConfigurator> configAction)
        {
            var endpointFactory = EndpointFactoryConfigurator.New(x =>
            {
                x.SetObjectBuilder(Kernel.Get<IObjectBuilder>());
                configAction(x);
            });

            Bind<IEndpointFactory>()
                .ToConstant(endpointFactory)
                .InSingletonScope()
                .Named("endpointFactory");
        }

        //at least one of these
        public void AddBus(string id, string endpoint, string subscriptionEndPoint)
        {
            RegisterServiceBus(id, endpoint, x =>
            {
                ConfigureThreadingModel(x);
                ConfigureSubscriptionClient(subscriptionEndPoint, x);
                ConfigureManagementClient(x);
                ConfigureControlBus(id, endpoint, x);
            });
        }

        private void RegisterServiceBus(string id, string endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            var bus = ServiceBusConfigurator.New(x =>
            {
                x.ReceiveFrom(endpointUri);
                configAction(x);
            });

            Bind<IServiceBus>()
                .ToConstant(bus)
                .InSingletonScope()
                .Named(id);
        }

        private void ConfigureThreadingModel(IServiceBusConfigurator configurator)
        {
            configurator.SetConcurrentConsumerLimit(ConcurrentConsumerLimit);
            configurator.SetConcurrentReceiverLimit(ConcurrentReceiverLimit);
        }

        private void ConfigureSubscriptionClient(string subscriptionEndPoint, IServiceBusConfigurator configurator)
        {
            configurator.ConfigureService<SubscriptionClientConfigurator>(x => x.SetSubscriptionServiceEndpoint(subscriptionEndPoint));
        }

        private void ConfigureManagementClient(IServiceBusConfigurator configurator)
        {
            configurator.ConfigureService<HealthClientConfigurator>(x => x.SetHeartbeatInterval(3));
        }

        private void ConfigureControlBus(string id, string endpoint, IServiceBusConfigurator configurator)
        {
            var bus = RegisterControlBus(id, endpoint, ConfigureThreadingModel);
            configurator.UseControlBus(bus);
        }

        private IControlBus RegisterControlBus(string id, string endpointUri, Action<IServiceBusConfigurator> configAction)
        {
            var bus = ControlBusConfigurator.New(x =>
            {
                x.ReceiveFrom(endpointUri);
                x.SetConcurrentReceiverLimit(1);
                configAction(x);
            });

            Bind<IControlBus>()
                .ToConstant(bus)
                .InSingletonScope()
                .Named(id);

            return bus;
        }

    }
}
