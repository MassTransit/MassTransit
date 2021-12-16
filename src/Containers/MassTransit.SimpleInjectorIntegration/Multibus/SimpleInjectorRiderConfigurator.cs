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
            throw new NotImplementedException("Rider not supported in Simple Injector");
        }
    }
}
