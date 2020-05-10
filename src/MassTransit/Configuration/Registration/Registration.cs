namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConfigurators;
    using Definition;
    using Metadata;
    using Saga;


    public class Registration :
        IRegistration
    {
        readonly IConfigurationServiceProvider _configurationServiceProvider;
        readonly IDictionary<Type, IConsumerRegistration> _consumers;
        readonly IDictionary<Type, ISagaRegistration> _sagas;
        readonly IDictionary<Type, IExecuteActivityRegistration> _executeActivities;
        readonly IDictionary<Type, IActivityRegistration> _activities;
        readonly IDictionary<Type, IEndpointRegistration> _endpoints;

        public Registration(IConfigurationServiceProvider configurationServiceProvider, IDictionary<Type, IConsumerRegistration> consumers,
            IDictionary<Type, ISagaRegistration> sagas, IDictionary<Type, IExecuteActivityRegistration> executeActivities,
            IDictionary<Type, IActivityRegistration> activities, IDictionary<Type, IEndpointRegistration> endpoints)
        {
            _configurationServiceProvider = configurationServiceProvider;
            _consumers = consumers;
            _sagas = sagas;
            _executeActivities = executeActivities;
            _activities = activities;
            _endpoints = endpoints;
        }

        public void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator)
        {
            if (!_consumers.TryGetValue(consumerType, out var consumer))
                throw new ArgumentException($"The consumer type was not found: {TypeMetadataCache.GetShortName(consumerType)}", nameof(consumerType));

            consumer.Configure(configurator, _configurationServiceProvider);
        }

        public void ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer
        {
            if (!_consumers.TryGetValue(typeof(T), out var consumer))
                throw new ArgumentException($"The consumer type was not found: {TypeMetadataCache.GetShortName(typeof(T))}", nameof(T));

            consumer.AddConfigureAction(configure);
            consumer.Configure(configurator, _configurationServiceProvider);
        }

        public void ConfigureConsumers(IReceiveEndpointConfigurator configurator)
        {
            foreach (var consumer in _consumers.Values)
                consumer.Configure(configurator, _configurationServiceProvider);
        }

        public void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator)
        {
            if (!_sagas.TryGetValue(sagaType, out var saga))
                throw new ArgumentException($"The saga type was not found: {TypeMetadataCache.GetShortName(sagaType)}", nameof(sagaType));

            saga.Configure(configurator, _configurationServiceProvider);
        }

        public void ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            if (!_sagas.TryGetValue(typeof(T), out var saga))
                throw new ArgumentException($"The saga type was not found: {TypeMetadataCache.GetShortName(typeof(T))}", nameof(T));

            saga.AddConfigureAction(configure);
            saga.Configure(configurator, _configurationServiceProvider);
        }

        public void ConfigureSagas(IReceiveEndpointConfigurator configurator)
        {
            foreach (var saga in _sagas.Values)
                saga.Configure(configurator, _configurationServiceProvider);
        }

        public void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
        {
            if (!_executeActivities.TryGetValue(activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            activity.Configure(configurator, _configurationServiceProvider);
        }

        public void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (!_activities.TryGetValue(activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            activity.Configure(executeEndpointConfigurator, compensateEndpointConfigurator, _configurationServiceProvider);
        }

        public void ConfigureActivityExecute(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, Uri compensateAddress)
        {
            if (!_activities.TryGetValue(activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            activity.ConfigureExecute(executeEndpointConfigurator, _configurationServiceProvider, compensateAddress);
        }

        public void ConfigureActivityCompensate(Type activityType, IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (!_activities.TryGetValue(activityType, out var activity))
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            activity.ConfigureCompensate(compensateEndpointConfigurator, _configurationServiceProvider);
        }

        public void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter endpointNameFormatter)
            where T : IReceiveEndpointConfigurator
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            endpointNameFormatter ??= _configurationServiceProvider.GetService<IEndpointNameFormatter>()
                ?? DefaultEndpointNameFormatter.Instance;

            var consumersByEndpoint = _consumers.Values
                .Select(x => x.GetDefinition(_configurationServiceProvider))
                .GroupBy(x => x.GetEndpointName(endpointNameFormatter));

            var sagasByEndpoint = _sagas.Values
                .Select(x => x.GetDefinition(_configurationServiceProvider))
                .GroupBy(x => x.GetEndpointName(endpointNameFormatter));

            var activities = _activities.Values
                .Select(x => x.GetDefinition(_configurationServiceProvider))
                .ToArray();

            var activitiesByExecuteEndpoint = activities
                .GroupBy(x => x.GetExecuteEndpointName(endpointNameFormatter));

            var activitiesByCompensateEndpoint = activities
                .GroupBy(x => x.GetCompensateEndpointName(endpointNameFormatter));

            var executeActivitiesByEndpoint = _executeActivities.Values
                .Select(x => x.GetDefinition(_configurationServiceProvider))
                .GroupBy(x => x.GetExecuteEndpointName(endpointNameFormatter));

            var endpointsWithName = _endpoints.Values
                .Select(x => x.GetDefinition(_configurationServiceProvider))
                .Select(x => new
                {
                    Definition = x,
                    Name = x.GetEndpointName(endpointNameFormatter)
                })
                .GroupBy(x => x.Name, (name, values) => new
                {
                    Name = name,
                    Definition = values.Select(x => x.Definition).Combine()
                });

            var endpointNames = consumersByEndpoint.Select(x => x.Key)
                .Union(sagasByEndpoint.Select(x => x.Key))
                .Union(activitiesByExecuteEndpoint.Select(x => x.Key))
                .Union(executeActivitiesByEndpoint.Select(x => x.Key))
                .Union(endpointsWithName.Select(x => x.Name))
                .Except(activitiesByCompensateEndpoint.Select(x => x.Key));

            var endpoints =
                from e in endpointNames
                join c in consumersByEndpoint on e equals c.Key into cs
                from c in cs.DefaultIfEmpty()
                join s in sagasByEndpoint on e equals s.Key into ss
                from s in ss.DefaultIfEmpty()
                join a in activitiesByExecuteEndpoint on e equals a.Key into aes
                from a in aes.DefaultIfEmpty()
                join ea in executeActivitiesByEndpoint on e equals ea.Key into eas
                from ea in eas.DefaultIfEmpty()
                join ep in endpointsWithName on e equals ep.Name into eps
                from ep in eps.Select(x => x.Definition)
                    .DefaultIfEmpty(c?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x)).Combine()
                        ?? s?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x)).Combine()
                        ?? a?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x)).Combine()
                        ?? ea?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x)).Combine()
                        ?? new NamedEndpointDefinition(e))
                select new
                {
                    Name = e,
                    Definition = ep,
                    Consumers = c,
                    Sagas = s,
                    Activities = a,
                    ExecuteActivities = ea
                };

            foreach (var endpoint in endpoints)
            {
                configurator.ReceiveEndpoint(endpoint.Definition, endpointNameFormatter, cfg =>
                {
                    if (endpoint.Consumers != null)
                        foreach (var consumer in endpoint.Consumers)
                        {
                            ConfigureConsumer(consumer.ConsumerType, cfg);
                        }

                    if (endpoint.Sagas != null)
                        foreach (var saga in endpoint.Sagas)
                        {
                            ConfigureSaga(saga.SagaType, cfg);
                        }

                    if (endpoint.Activities != null)
                        foreach (var activity in endpoint.Activities)
                        {
                            var compensateEndpointName = activity.GetCompensateEndpointName(endpointNameFormatter);

                            var compensateDefinition = endpointsWithName.SingleOrDefault(x => x.Name == compensateEndpointName)?.Definition;
                            if (compensateDefinition != null)
                            {
                                configurator.ReceiveEndpoint(compensateDefinition, endpointNameFormatter, compensateEndpointConfigurator =>
                                {
                                    ConfigureActivity(activity.ActivityType, cfg, compensateEndpointConfigurator);
                                });
                            }
                            else
                            {
                                configurator.ReceiveEndpoint(compensateEndpointName, compensateEndpointConfigurator =>
                                {
                                    ConfigureActivity(activity.ActivityType, cfg, compensateEndpointConfigurator);
                                });
                            }
                        }

                    if (endpoint.ExecuteActivities != null)
                        foreach (var activity in endpoint.ExecuteActivities)
                        {
                            ConfigureExecuteActivity(activity.ActivityType, cfg);
                        }
                });
            }
        }
    }
}
