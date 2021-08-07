namespace MassTransit.AutofacIntegration.MultiBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using MassTransit.MultiBus;
    using MassTransit.Registration;
    using Monitoring.Health;
    using Registration;
    using Scoping;
    using Transports;


    public class ContainerBuilderBusConfigurator<TBus, TBusInstance> :
        ContainerBuilderBusRegistrationConfigurator,
        IContainerBuilderBusConfigurator<TBus>
        where TBus : class, IBus
        where TBusInstance : BusInstance<TBus>, TBus
    {
        public ContainerBuilderBusConfigurator(ContainerBuilder builder)
            : base(builder, new AutofacContainerRegistrar<TBus>(builder))
        {
            IBusRegistrationContext CreateRegistrationContext(IComponentContext context)
            {
                var provider = context.Resolve<IConfigurationServiceProvider>();
                return new BusRegistrationContext(provider, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
            }

            Builder.Register(context => Bind<TBus>.Create(GetSendEndpointProvider(context)))
                .As<Bind<TBus, ISendEndpointProvider>>()
                .InstancePerLifetimeScope();

            Builder.Register(context => Bind<TBus>.Create(GetPublishEndpoint(context)))
                .As<Bind<TBus, IPublishEndpoint>>()
                .InstancePerLifetimeScope();

            Builder.Register(context =>
                {
                    var provider = context.Resolve<IConfigurationServiceProvider>();
                    var bus = context.Resolve<TBus>();
                    return Bind<TBus>.Create(ClientFactoryProvider(provider, bus));
                })
                .As<Bind<TBus, IClientFactory>>()
                .SingleInstance();

            Builder.Register(context => Bind<TBus>.Create(CreateRegistrationContext(context)))
                .As<Bind<TBus, IBusRegistrationContext>>()
                .SingleInstance();
        }

        public override void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            SetBusFactory(new RegistrationBusFactory(busFactory));
        }

        public override void SetBusFactory<T>(T busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusFactory));

            Builder.Register(context => CreateBus(busFactory, context))
                .As<IBusInstance<TBus>>()
                .As<IBusInstance>()
                .SingleInstance();

            Builder.Register(context => Bind<TBus>.Create(context.Resolve<IBusInstance<TBus>>()))
                .As<Bind<TBus, IBusInstance<TBus>>>()
                .SingleInstance();

            Builder.Register(context => Bind<TBus>.Create<IReceiveEndpointConnector>(context.Resolve<IBusInstance<TBus>>()))
                .As<Bind<TBus, IReceiveEndpointConnector>>()
                .SingleInstance();

            Builder.Register(context => context.Resolve<IBusInstance<TBus>>().BusInstance)
                .As<TBus>()
                .SingleInstance();


        #pragma warning disable 618
            Builder.Register(context => new BusHealth(context.Resolve<IBusInstance<TBus>>()))
                .As<IBusHealth>()
                .SingleInstance();
        }

        public override void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            var configurator = new ContainerBuilderRiderConfigurator<TBus>(Builder, Registrar, RiderTypes);
            configure?.Invoke(configurator);
        }

        static IBusInstance<TBus> CreateBus<T>(T busFactory, IComponentContext context)
            where T : IRegistrationBusFactory
        {
            IEnumerable<IBusInstanceSpecification> specifications = context.Resolve<IEnumerable<Bind<TBus, IBusInstanceSpecification>>>().Select(x => x.Value);

            var instance = busFactory.CreateBus(context.Resolve<Bind<TBus, IBusRegistrationContext>>().Value, specifications);

            var busInstance = context.ResolveOptional<TBusInstance>()
                ?? (TBusInstance)Activator.CreateInstance(typeof(TBusInstance), instance.BusControl);

            return new MultiBusInstance<TBus>(busInstance, instance);
        }

        static ISendEndpointProvider GetSendEndpointProvider(IComponentContext context)
        {
            return new ScopedSendEndpointProvider<ILifetimeScope>(context.Resolve<TBus>(), context.Resolve<ILifetimeScope>());
        }

        static IPublishEndpoint GetPublishEndpoint(IComponentContext context)
        {
            return new PublishEndpoint(new ScopedPublishEndpointProvider<ILifetimeScope>(context.Resolve<TBus>(), context.Resolve<ILifetimeScope>()));
        }
    }
}
