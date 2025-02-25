namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;


    public class RegistrationContext :
        IRegistrationContext,
        ISetScopedConsumeContext
    {
        readonly HashSet<Type> _configuredTypes;
        readonly IServiceProvider _provider;
        readonly ISetScopedConsumeContext _setScopedConsumeContext;

        public RegistrationContext(IServiceProvider provider, IContainerSelector selector, ISetScopedConsumeContext setScopedConsumeContext)
        {
            Selector = selector;
            _provider = provider;
            _setScopedConsumeContext = setScopedConsumeContext;

            _configuredTypes = new HashSet<Type>();
        }

        protected IContainerSelector Selector { get; }

        public void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator)
        {
            if (!Selector.TryGetRegistration<IConsumerRegistration>(_provider, consumerType, out var consumer))
                throw new ArgumentException($"The consumer type was not found: {TypeCache.GetShortName(consumerType)}", nameof(consumerType));

            consumer.Configure(configurator, this);
            _configuredTypes.Add(consumerType);
        }

        public void ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer
        {
            if (!Selector.TryGetRegistration<IConsumerRegistration>(_provider, typeof(T), out var consumer))
                throw new ArgumentException($"The consumer type was not found: {TypeCache.GetShortName(typeof(T))}", nameof(T));

            if (configure != null)
                consumer.AddConfigureAction<T>((_, cfg) => configure.Invoke(cfg));

            consumer.Configure(configurator, this);
            _configuredTypes.Add(typeof(T));
        }

        public void ConfigureConsumers(IReceiveEndpointConfigurator configurator)
        {
            foreach (var consumer in Selector.GetRegistrations<IConsumerRegistration>(_provider).Where(x => x.IncludeInConfigureEndpoints))
            {
                consumer.Configure(configurator, this);
                _configuredTypes.Add(consumer.Type);
            }
        }

        public void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator)
        {
            if (!Selector.TryGetRegistration<ISagaRegistration>(_provider, sagaType, out var saga))
                throw new ArgumentException($"The saga type was not found: {TypeCache.GetShortName(sagaType)}", nameof(sagaType));

            saga.Configure(configurator, this);
            _configuredTypes.Add(sagaType);
        }

        public void ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            if (!Selector.TryGetRegistration<ISagaRegistration>(_provider, typeof(T), out var saga))
                throw new ArgumentException($"The saga type was not found: {TypeCache.GetShortName(typeof(T))}", nameof(T));

            if (configure != null)
                saga.AddConfigureAction<T>((_, cfg) => configure.Invoke(cfg));

            saga.Configure(configurator, this);
            _configuredTypes.Add(typeof(T));
        }

        public void ConfigureSagas(IReceiveEndpointConfigurator configurator)
        {
            foreach (var saga in Selector.GetRegistrations<ISagaRegistration>(_provider).Where(x => x.IncludeInConfigureEndpoints))
            {
                saga.Configure(configurator, this);
                _configuredTypes.Add(saga.Type);
            }
        }

        public void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
        {
            if (!Selector.TryGetRegistration<IExecuteActivityRegistration>(_provider, activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            activity.Configure(configurator, this);
            _configuredTypes.Add(activityType);
        }

        public void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (!Selector.TryGetRegistration<IActivityRegistration>(_provider, activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            activity.Configure(executeEndpointConfigurator, compensateEndpointConfigurator, this);
            _configuredTypes.Add(activityType);
        }

        public void ConfigureActivityExecute(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, Uri compensateAddress)
        {
            if (!Selector.TryGetRegistration<IActivityRegistration>(_provider, activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            activity.ConfigureExecute(executeEndpointConfigurator, this, compensateAddress);
            _configuredTypes.Add(activityType);
        }

        public void ConfigureActivityCompensate(Type activityType, IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (!Selector.TryGetRegistration<IActivityRegistration>(_provider, activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            activity.ConfigureCompensate(compensateEndpointConfigurator, this);
            _configuredTypes.Add(activityType);
        }

        public void ConfigureFuture(Type futureType, IReceiveEndpointConfigurator configurator)
        {
            if (!Selector.TryGetRegistration<IFutureRegistration>(_provider, futureType, out var future))
                throw new ArgumentException($"The future type was not found: {TypeCache.GetShortName(futureType)}", nameof(futureType));

            future.Configure(configurator, this);
            _configuredTypes.Add(futureType);
        }

        public void ConfigureFuture<T>(IReceiveEndpointConfigurator configurator)
            where T : class, ISaga
        {
            if (!Selector.TryGetRegistration<IFutureRegistration>(_provider, typeof(T), out var future))
                throw new ArgumentException($"The future type was not found: {TypeCache.GetShortName(typeof(T))}", nameof(T));

            future.Configure(configurator, this);
            _configuredTypes.Add(typeof(T));
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IContainerSelector))
                return Selector;

            return _provider.GetService(serviceType);
        }

        public IDisposable PushContext(IServiceScope scope, ConsumeContext context)
        {
            return _setScopedConsumeContext.PushContext(scope, context);
        }

        protected bool WasConfigured(Type type)
        {
            return _configuredTypes.Contains(type);
        }
    }
}
