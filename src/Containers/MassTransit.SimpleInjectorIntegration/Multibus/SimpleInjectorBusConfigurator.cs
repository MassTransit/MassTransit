using MassTransit.MultiBus;
using MassTransit.SimpleInjectorIntegration.Registration;
using SimpleInjector;
using System;
using System.Collections.Generic;

namespace MassTransit.SimpleInjectorIntegration.Multibus
{
    using System.Linq;
    using MassTransit.Registration;
    using Monitoring.Health;
    using Scoping;
    using Transports;


    public class SimpleInjectorBusConfigurator<TBus, TBusInstance> :
        SimpleInjectorBusConfigurator,
        ISimpleInjectorBusConfigurator<TBus>
        where TBus : class, IBus
        where TBusInstance : BusInstance<TBus>, TBus
    {
        public SimpleInjectorBusConfigurator(Container container)
            : base(container, new SimpleInjectorContainerRegistrar<TBus>(container))
        {
            IBusRegistrationContext CreateRegistrationContext()
            {
                var provider = Container.GetInstance<IConfigurationServiceProvider>();
                return new BusRegistrationContext(provider, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
            }

            RegisterBusDepot();

            Container.Collection.Register(new Bind<TBus, IBusInstanceSpecification>[]{ });
            Container.Register(() => Bind<TBus>.Create(GetSendEndpointProvider(Container)), Lifestyle.Scoped);

            Container.Register(() => Bind<TBus>.Create(GetPublishEndpoint(Container)), Lifestyle.Scoped);

            Container.RegisterSingleton(() =>
            {
                var provider = Container.GetInstance<IConfigurationServiceProvider>();
                var bus = Container.GetInstance<TBus>();
                return Bind<TBus>.Create(ClientFactoryProvider(provider, bus));
            });

            Container.RegisterSingleton(() => Bind<TBus>.Create(CreateRegistrationContext()));
        }

        void RegisterBusDepot()
        {
            try
            {
                // there is no way to determine whether a registration already exists in Simple Injector
                Container.RegisterSingleton<IBusDepot, BusDepot>();
            }
            catch (InvalidOperationException e)
            {
                if (!e.Message.Contains("Type IBusDepot has already been registered"))
                    throw;
            }
        }

        public override void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            var configurator = new SimpleInjectorRiderConfigurator<TBus>(Container, Registrar, RiderTypes);
            configure?.Invoke(configurator);
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

            Container.RegisterSingleton(() => CreateBus(busFactory));
            Container.Collection.Append(
                    () => (IBusInstance) Container.GetInstance<IBusInstance<TBus>>()
                    , Lifestyle.Singleton
            );

            Container.RegisterSingleton(() => Bind<TBus>.Create(Container.GetInstance<IBusInstance<TBus>>()));
            Container.RegisterSingleton(() => Bind<TBus>.Create<IReceiveEndpointConnector>(Container.GetInstance<IBusInstance<TBus>>()));
            Container.RegisterSingleton(() => Container.GetInstance<IBusInstance<TBus>>().BusInstance);

            Registrar.RegisterScopedClientFactory();

        #pragma warning disable 618
            var doesIBusHealthExist = Container.GetCurrentRegistrations().SingleOrDefault(r => r.Registration.ImplementationType == typeof(IBusHealth)) != null;
            if(!doesIBusHealthExist)
                Container.RegisterSingleton<IBusHealth>(() => new BusHealth(Container.GetInstance<IBusInstance<TBus>>()));
        }

        IBusInstance<TBus> CreateBus<T>(T busFactory)
            where T : IRegistrationBusFactory
        {
            IEnumerable<IBusInstanceSpecification> specifications =
                Container.GetInstance<IEnumerable<Bind<TBus, IBusInstanceSpecification>>>().Select(x => x.Value);

            var instance = busFactory.CreateBus(Container.GetInstance<Bind<TBus, IBusRegistrationContext>>().Value, specifications);
            var busInstance = Container.TryGetInstance<TBusInstance>() ?? (TBusInstance)Activator.CreateInstance(typeof(TBusInstance), instance.BusControl);

            return new MultiBusInstance<TBus>(busInstance, instance);
        }

        static ISendEndpointProvider GetSendEndpointProvider(Container container)
        {
            return new ScopedSendEndpointProvider<IServiceProvider>(container.GetInstance<TBus>(), container);
        }

        static IPublishEndpoint GetPublishEndpoint(Container container)
        {
            return new PublishEndpoint(new ScopedPublishEndpointProvider<IServiceProvider>(container.GetInstance<TBus>(), container));
        }
    }
}
