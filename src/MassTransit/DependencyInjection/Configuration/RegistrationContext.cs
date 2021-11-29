namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class RegistrationContext :
        IRegistrationContext
    {
        readonly HashSet<Type> _configuredTypes;
        readonly IServiceProvider _provider;

        public RegistrationContext(IServiceProvider provider, IContainerSelector selector)
        {
            Selector = selector;
            _provider = provider;

            _configuredTypes = new HashSet<Type>();
        }

        protected IContainerSelector Selector { get; }

        public void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator)
        {
            if (!Selector.TryGetValue<IConsumerRegistration>(_provider, consumerType, out var consumer))
                throw new ArgumentException($"The consumer type was not found: {TypeCache.GetShortName(consumerType)}", nameof(consumerType));

            consumer.Configure(configurator, this);

            _configuredTypes.Add(consumerType);
        }

        public void ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer
        {
            if (!Selector.TryGetValue<IConsumerRegistration>(_provider, typeof(T), out var consumer))
                throw new ArgumentException($"The consumer type was not found: {TypeCache.GetShortName(typeof(T))}", nameof(T));

            consumer.AddConfigureAction(configure);
            consumer.Configure(configurator, this);

            _configuredTypes.Add(typeof(T));
        }

        public void ConfigureConsumers(IReceiveEndpointConfigurator configurator)
        {
            foreach (var consumer in Selector.GetRegistrations<IConsumerRegistration>(_provider).Where(x => !WasConfigured(x.Type)))
            {
                consumer.Configure(configurator, this);

                _configuredTypes.Add(consumer.Type);
            }
        }

        public void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator)
        {
            if (!Selector.TryGetValue<ISagaRegistration>(_provider, sagaType, out var saga))
                throw new ArgumentException($"The saga type was not found: {TypeCache.GetShortName(sagaType)}", nameof(sagaType));

            saga.Configure(configurator, this);

            _configuredTypes.Add(sagaType);
        }

        public void ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            if (!Selector.TryGetValue<ISagaRegistration>(_provider, typeof(T), out var saga))
                throw new ArgumentException($"The saga type was not found: {TypeCache.GetShortName(typeof(T))}", nameof(T));

            saga.AddConfigureAction(configure);
            saga.Configure(configurator, this);

            _configuredTypes.Add(typeof(T));
        }

        public void ConfigureSagas(IReceiveEndpointConfigurator configurator)
        {
            foreach (var saga in Selector.GetRegistrations<ISagaRegistration>(_provider).Where(x => !WasConfigured(x.Type)))
            {
                saga.Configure(configurator, this);

                _configuredTypes.Add(saga.Type);
            }
        }

        public void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
        {
            if (!Selector.TryGetValue<IExecuteActivityRegistration>(_provider, activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            activity.Configure(configurator, this);

            _configuredTypes.Add(activityType);
        }

        public void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (!Selector.TryGetValue<IActivityRegistration>(_provider, activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            activity.Configure(executeEndpointConfigurator, compensateEndpointConfigurator, this);

            _configuredTypes.Add(activityType);
        }

        public void ConfigureActivityExecute(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, Uri compensateAddress)
        {
            if (!Selector.TryGetValue<IActivityRegistration>(_provider, activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            activity.ConfigureExecute(executeEndpointConfigurator, this, compensateAddress);

            _configuredTypes.Add(activityType);
        }

        public void ConfigureActivityCompensate(Type activityType, IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (!Selector.TryGetValue<IActivityRegistration>(_provider, activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            activity.ConfigureCompensate(compensateEndpointConfigurator, this);

            _configuredTypes.Add(activityType);
        }

        public void ConfigureFuture(Type futureType, IReceiveEndpointConfigurator configurator)
        {
            if (!Selector.TryGetValue<IFutureRegistration>(_provider, futureType, out var future))
                throw new ArgumentException($"The future type was not found: {TypeCache.GetShortName(futureType)}", nameof(futureType));

            future.Configure(configurator, this);

            _configuredTypes.Add(futureType);
        }

        public void ConfigureFuture<T>(IReceiveEndpointConfigurator configurator)
            where T : class, ISaga
        {
            if (!Selector.TryGetValue<IFutureRegistration>(_provider, typeof(T), out var future))
                throw new ArgumentException($"The future type was not found: {TypeCache.GetShortName(typeof(T))}", nameof(T));

            future.Configure(configurator, this);

            _configuredTypes.Add(typeof(T));
        }

        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }

        protected bool WasConfigured(Type type)
        {
            return _configuredTypes.Contains(type);
        }
    }
}
