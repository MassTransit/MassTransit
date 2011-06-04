namespace MassTransit.WindsorIntegration
{
    using System;
    using Castle.Windsor;
    using Magnum.Reflection;
    using Saga;
    using SubscriptionConfigurators;
    using Util;

    public class WindsorSagaFactoryConfigurator
    {
        private readonly SubscriptionBusServiceConfigurator _configurator;
        private readonly IWindsorContainer _container;

        public WindsorSagaFactoryConfigurator(SubscriptionBusServiceConfigurator configurator,
                                              IWindsorContainer container)
        {
            _container = container;
            _configurator = configurator;
        }

        public void ConfigureSaga(Type messageType)
        {
            this.FastInvoke(new[] {messageType}, "Configure");
        }

        [UsedImplicitly]
        public void Configure<T>()
            where T : class, ISaga
        {
            _configurator.Saga((ISagaRepository<T>) _container.Resolve(typeof (ISagaRepository<T>)));
        }
    }
}