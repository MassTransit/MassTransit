namespace MassTransit.AutofacIntegration.MultiBus
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using MassTransit.Registration;
    using Registration;


    public class ContainerBuilderRiderConfigurator<TBus> :
        ContainerBuilderRiderConfigurator,
        IContainerBuilderRiderConfigurator<TBus>
        where TBus : class, IBus
    {
        public ContainerBuilderRiderConfigurator(ContainerBuilder builder, IContainerRegistrar registrar, HashSet<Type> riderTypes)
            : base(builder, registrar, riderTypes)
        {
        }

        public override void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured<TRider>();

            IRiderRegistrationContext CreateRegistrationContext(IComponentContext context)
            {
                var registration = CreateRegistration(context.Resolve<IConfigurationServiceProvider>());
                return new RiderRegistrationContext(registration, Registrations);
            }

            Builder.Register(provider => Bind<TBus, TRider>.Create(CreateRegistrationContext(provider)))
                .SingleInstance();

            Builder.Register(provider =>
                    Bind<TBus>.Create(riderFactory.CreateRider(provider.Resolve<Bind<TBus, TRider, IRiderRegistrationContext>>().Value)))
                .SingleInstance();

            Builder.Register(provider => Bind<TBus>.Create(provider.Resolve<IBusInstance<TBus>>().GetRider<TRider>()))
                .SingleInstance();
        }
    }
}
