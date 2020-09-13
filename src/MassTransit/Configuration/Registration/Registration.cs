namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using ConsumeConfigurators;
    using Metadata;
    using Saga;


    public class Registration :
        IRegistration
    {
        readonly IConfigurationServiceProvider _configurationServiceProvider;
        readonly HashSet<Type> _configuredTypes;

        protected readonly IRegistrationCache<IActivityRegistration> Activities;
        protected readonly IRegistrationCache<IConsumerRegistration> Consumers;
        protected readonly IRegistrationCache<IExecuteActivityRegistration> ExecuteActivities;
        protected readonly IRegistrationCache<ISagaRegistration> Sagas;

        public Registration(IConfigurationServiceProvider configurationServiceProvider, IRegistrationCache<IConsumerRegistration> consumers,
            IRegistrationCache<ISagaRegistration> sagas, IRegistrationCache<IExecuteActivityRegistration> executeActivities,
            IRegistrationCache<IActivityRegistration> activities)
        {
            _configurationServiceProvider = configurationServiceProvider;
            Consumers = consumers;
            Sagas = sagas;
            ExecuteActivities = executeActivities;
            Activities = activities;

            _configuredTypes = new HashSet<Type>();
        }

        public void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator)
        {
            if (!Consumers.TryGetValue(consumerType, out var consumer))
                throw new ArgumentException($"The consumer type was not found: {TypeMetadataCache.GetShortName(consumerType)}", nameof(consumerType));

            consumer.Configure(configurator, this);

            _configuredTypes.Add(consumerType);
        }

        public void ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer
        {
            if (!Consumers.TryGetValue(typeof(T), out var consumer))
                throw new ArgumentException($"The consumer type was not found: {TypeMetadataCache.GetShortName(typeof(T))}", nameof(T));

            consumer.AddConfigureAction(configure);
            consumer.Configure(configurator, this);

            _configuredTypes.Add(typeof(T));
        }

        public void ConfigureConsumers(IReceiveEndpointConfigurator configurator)
        {
            foreach (var consumer in Consumers.Values)
            {
                consumer.Configure(configurator, this);

                _configuredTypes.Add(consumer.ConsumerType);
            }
        }

        public void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator)
        {
            if (!Sagas.TryGetValue(sagaType, out var saga))
                throw new ArgumentException($"The saga type was not found: {TypeMetadataCache.GetShortName(sagaType)}", nameof(sagaType));

            saga.Configure(configurator, this);

            _configuredTypes.Add(sagaType);
        }

        public void ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            if (!Sagas.TryGetValue(typeof(T), out var saga))
                throw new ArgumentException($"The saga type was not found: {TypeMetadataCache.GetShortName(typeof(T))}", nameof(T));

            saga.AddConfigureAction(configure);
            saga.Configure(configurator, this);

            _configuredTypes.Add(typeof(T));
        }

        public void ConfigureSagas(IReceiveEndpointConfigurator configurator)
        {
            foreach (var saga in Sagas.Values)
            {
                saga.Configure(configurator, this);

                _configuredTypes.Add(saga.SagaType);
            }
        }

        public void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
        {
            if (!ExecuteActivities.TryGetValue(activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            activity.Configure(configurator, this);

            _configuredTypes.Add(activityType);
        }

        public void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (!Activities.TryGetValue(activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            activity.Configure(executeEndpointConfigurator, compensateEndpointConfigurator, this);

            _configuredTypes.Add(activityType);
        }

        public void ConfigureActivityExecute(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, Uri compensateAddress)
        {
            if (!Activities.TryGetValue(activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            activity.ConfigureExecute(executeEndpointConfigurator, this, compensateAddress);

            _configuredTypes.Add(activityType);
        }

        public void ConfigureActivityCompensate(Type activityType, IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (!Activities.TryGetValue(activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            activity.ConfigureCompensate(compensateEndpointConfigurator, this);

            _configuredTypes.Add(activityType);
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return _configurationServiceProvider.GetService(serviceType);
        }

        T IConfigurationServiceProvider.GetRequiredService<T>()
            where T : class
        {
            return _configurationServiceProvider.GetRequiredService<T>();
        }

        public T GetService<T>()
            where T : class
        {
            return _configurationServiceProvider.GetService<T>();
        }

        protected bool WasConfigured(Type type)
        {
            return _configuredTypes.Contains(type);
        }
    }
}
