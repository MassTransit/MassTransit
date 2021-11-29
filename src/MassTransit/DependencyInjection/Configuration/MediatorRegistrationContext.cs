namespace MassTransit.Configuration
{
    using System;


    public class MediatorRegistrationContext :
        IMediatorRegistrationContext
    {
        readonly IRegistrationContext _registration;

        public MediatorRegistrationContext(IRegistrationContext registration)
        {
            _registration = registration;
        }

        public object GetService(Type serviceType)
        {
            return _registration.GetService(serviceType);
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

        public void ConfigureFuture(Type futureType, IReceiveEndpointConfigurator configurator)
        {
            _registration.ConfigureFuture(futureType, configurator);
        }

        public void ConfigureFuture<T>(IReceiveEndpointConfigurator configurator)
            where T : class, ISaga
        {
            _registration.ConfigureFuture<T>(configurator);
        }
    }
}
