namespace MassTransit.Registration
{
    using System;
    using ConsumeConfigurators;
    using Monitoring.Health;
    using Saga;


    public class RegistrationContext<TContainerContext> :
        IRegistrationContext<TContainerContext>
        where TContainerContext : class
    {
        readonly IRegistration _registration;
        readonly BusHealth _busHealth;

        public RegistrationContext(IRegistration registration, BusHealth busHealth, TContainerContext container)
        {
            Container = container;

            _registration = registration;
            _busHealth = busHealth;
        }

        public TContainerContext Container { get; }

        public void UseHealthCheck(IBusFactoryConfigurator configurator)
        {
            configurator.ConnectBusObserver(_busHealth);
            configurator.ConnectEndpointConfigurationObserver(_busHealth);
        }

        public void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator)
        {
            _registration.ConfigureConsumer(consumerType, configurator);
        }

        public void ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            _registration.ConfigureConsumer(configurator, configure);
        }

        public void ConfigureConsumers(IReceiveEndpointConfigurator configurator)
        {
            _registration.ConfigureConsumers(configurator);
        }

        public void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator)
        {
            _registration.ConfigureSaga(sagaType, configurator);
        }

        public void ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            _registration.ConfigureSaga(configurator, configure);
        }

        public void ConfigureSagas(IReceiveEndpointConfigurator configurator)
        {
            _registration.ConfigureSagas(configurator);
        }

        public void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
        {
            _registration.ConfigureExecuteActivity(activityType, configurator);
        }

        public void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            _registration.ConfigureActivity(activityType, executeEndpointConfigurator, compensateEndpointConfigurator);
        }

        public void ConfigureActivityExecute(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, Uri compensateAddress)
        {
            _registration.ConfigureActivityExecute(activityType, executeEndpointConfigurator, compensateAddress);
        }

        public void ConfigureActivityCompensate(Type activityType, IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            _registration.ConfigureActivityCompensate(activityType, compensateEndpointConfigurator);
        }

        public void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter endpointNameFormatter = null)
            where T : IReceiveEndpointConfigurator
        {
            _registration.ConfigureEndpoints(configurator, endpointNameFormatter);
        }
    }
}
