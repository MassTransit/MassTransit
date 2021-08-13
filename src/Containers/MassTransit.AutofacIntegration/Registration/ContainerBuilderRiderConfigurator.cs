namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using Autofac.Core;
    using MassTransit.Registration;
    using Riders;


    public class ContainerBuilderRiderConfigurator :
        RegistrationConfigurator,
        IContainerBuilderRiderConfigurator
    {
        readonly HashSet<Type> _riderTypes;
        protected readonly RegistrationCache<object> Registrations;

        public ContainerBuilderRiderConfigurator(ContainerBuilder builder, IContainerRegistrar registrar, HashSet<Type> riderTypes)
            : base(registrar)
        {
            Builder = builder;
            _riderTypes = riderTypes;
            Registrations = new RegistrationCache<object>();
        }

        public ContainerBuilder Builder { get; }

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

            IRiderRegistrationContext CreateRegistrationContext(IComponentContext context)
            {
                var registration = CreateRegistration(context.Resolve<IConfigurationServiceProvider>());
                return new RiderRegistrationContext(registration, Registrations);
            }

            var registrationKey = typeof(TRider).Name;

            Builder.Register(CreateRegistrationContext)
                .Keyed<IRiderRegistrationContext>(registrationKey)
                .SingleInstance();
            Builder.Register(context => riderFactory.CreateRider(context.ResolveKeyed<IRiderRegistrationContext>(registrationKey)))
                .As<IBusInstanceSpecification>()
                .SingleInstance();
            Builder.Register(context => context.Resolve<Bind<IBus, IBusInstance>>().Value.GetRider<TRider>())
                .As<TRider>()
                .SingleInstance();
        }

        protected void ThrowIfAlreadyConfigured<TRider>()
            where TRider : IRider
        {
            ThrowIfAlreadyConfigured(nameof(SetRiderFactory));
            var riderType = typeof(TRider);
            if (!_riderTypes.Add(riderType))
                throw new ConfigurationException($"'{riderType.Name}' can be added only once.");

            //TODO: maybe support it at some point....
            if (Builder.ComponentRegistryBuilder.IsRegistered(new TypedService(riderType)))
                throw new ConfigurationException($"'{riderType.Name}' has been already registered.");
        }
    }
}
