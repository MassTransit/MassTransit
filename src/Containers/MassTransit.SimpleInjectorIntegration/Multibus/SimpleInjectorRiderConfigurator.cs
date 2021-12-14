namespace MassTransit.SimpleInjectorIntegration.Multibus
{
    using System;
    using System.Collections.Generic;
    using AutofacIntegration;
    using MassTransit.Registration;
    using Registration;
    using SimpleInjector;


    public class SimpleInjectorRiderConfigurator<TBus> :
        SimpleInjectorRiderConfigurator,
        ISimpleInjectorRiderConfigurator<TBus>
        where TBus : class, IBus
    {
        public SimpleInjectorRiderConfigurator(Container container, IContainerRegistrar registrar, HashSet<Type> riderTypes)
            : base(container, registrar, riderTypes)
        {
        }

        public override void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured<TRider>();

            IRiderRegistrationContext CreateRegistrationContext()
            {
                var registration = CreateRegistration(Container.GetInstance<IConfigurationServiceProvider>());
                return new RiderRegistrationContext(registration, Registrations);
            }

            Container.RegisterSingleton(() => Bind<TBus, TRider>.Create(CreateRegistrationContext()));
            Container.Collection.AppendInstance(
                Lifestyle.Singleton.CreateRegistration(() =>
                    Bind<TBus>.Create(riderFactory.CreateRider(Container.GetInstance<Bind<TBus, TRider, IRiderRegistrationContext>>().Value)),
                    Container)
            );
            Container.RegisterSingleton(() => Bind<TBus>.Create(Container.GetInstance<IBusInstance<TBus>>().GetRider<TRider>()));
        }
    }
}
