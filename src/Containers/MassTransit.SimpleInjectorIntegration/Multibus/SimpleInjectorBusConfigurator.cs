using MassTransit.MultiBus;
using MassTransit.SimpleInjectorIntegration.Registration;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.SimpleInjectorIntegration.Multibus
{
    using System.Linq;
    using Autofac;
    using AutofacIntegration;
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
                var provider = container.GetInstance<IConfigurationServiceProvider>();
                return new BusRegistrationContext(provider, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
            }

            Container.Register(() => Bind<TBus>.Create(GetSendEndpointProvider(Container)), Lifestyle.Scoped);

            Container.Register(() => Bind<TBus>.Create(GetPublishEndpoint(Container)), Lifestyle.Scoped);

            Container.Register(() =>
            {
                var provider = container.GetInstance<IConfigurationServiceProvider>();
                var bus = container.GetInstance<TBus>();
                return Bind<TBus>.Create(ClientFactoryProvider(provider, bus));
            });

            Container.Collection.AppendInstance(
                Lifestyle.Singleton.CreateRegistration(() => Bind<TBus>.Create(CreateRegistrationContext()), Container)
            );
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

            var bus = CreateBus(busFactory, Container);
            Container.RegisterSingleton(() => bus);
            Container.Collection.AppendInstance(
                Lifestyle.Singleton.CreateRegistration<IBusInstance>(() => bus, Container)
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

        static IBusInstance<TBus> CreateBus<T>(T busFactory, Container container)
            where T : IRegistrationBusFactory
        {
            IEnumerable<IBusInstanceSpecification> specifications =
                container.GetInstance<IEnumerable<Bind<TBus, IBusInstanceSpecification>>>().Select(x => x.Value);

            var instance = busFactory.CreateBus(container.GetInstance<Bind<TBus, IBusRegistrationContext>>().Value, specifications);

            var busInstance = container.TryGetInstance<TBusInstance>() ?? (TBusInstance)Activator.CreateInstance(typeof(TBusInstance), instance.BusControl);

            return new MultiBusInstance<TBus>(busInstance, instance);
        }

        static ISendEndpointProvider GetSendEndpointProvider(Container container)
        {
            return new ScopedSendEndpointProvider<ILifetimeScope>(container.GetInstance<TBus>(), container.GetInstance<ILifetimeScope>());
        }

        static IPublishEndpoint GetPublishEndpoint(Container container)
        {
            return new PublishEndpoint(new ScopedPublishEndpointProvider<ILifetimeScope>(container.GetInstance<TBus>(), container.GetInstance<ILifetimeScope>()));
        }
    }
}
