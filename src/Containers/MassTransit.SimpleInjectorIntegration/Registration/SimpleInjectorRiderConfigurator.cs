namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Registration;
    using Riders;


    public class SimpleInjectorRiderConfigurator :
        RegistrationConfigurator,
        ISimpleInjectorRiderConfigurator
    {
        readonly HashSet<Type> _riderTypes;
        protected readonly RegistrationCache<object> Registrations;
        public SimpleInjector.Container Container { get; }

        public SimpleInjectorRiderConfigurator(SimpleInjector.Container container, IContainerRegistrar registrar, HashSet<Type> riderTypes)
            : base(registrar)
        {
            Container = container;
            _riderTypes = riderTypes;
            Registrations = new RegistrationCache<object>();
        }

        public void AddRegistration<T>(T registration)
            where T : class
        {
            Registrations.GetOrAdd(typeof(T), _ => registration);
        }

        public virtual void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
            where TRider : class, IRider
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured<TRider>();

            IRiderRegistrationContext CreateRegistrationContext()
            {
                var registration = CreateRegistration(Container.GetInstance<IConfigurationServiceProvider>());
                return new RiderRegistrationContext(registration, Registrations);
            }

            Container.RegisterSingleton(() => Bind<IBus, TRider>.Create(CreateRegistrationContext()));
            Container.RegisterSingleton(() =>
                Bind<IBus>.Create(riderFactory.CreateRider(Container.GetInstance<Bind<IBus, TRider, IRiderRegistrationContext>>().Value)));
            Container.RegisterSingleton(() => Bind<IBus>.Create(Container.GetInstance<Bind<IBus, IBusInstance>>().Value.GetRider<TRider>()));
            Container.RegisterSingleton(() => Container.GetInstance<Bind<IBus, TRider>>().Value);
        }

        protected void ThrowIfAlreadyConfigured<TRider>()
            where TRider : IRider
        {
            ThrowIfAlreadyConfigured(nameof(SetRiderFactory));
            var riderType = typeof(TRider);
            if (!_riderTypes.Add(riderType))
                throw new ConfigurationException($"'{riderType.Name}' can be added only once.");

            //TODO: maybe support it at some point....
            if (Container.GetRegistration(riderType) != null)
                throw new ConfigurationException($"'{riderType.Name}' has been already registered.");
        }
    }
}
