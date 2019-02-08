// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConfigurators;
    using Definition;
    using Logging;
    using Saga;
    using Util;


    public class Registration :
        IRegistration
    {
        static readonly ILog _log = Logger.Get<Registration>();

        readonly IConfigurationServiceProvider _configurationServiceProvider;
        readonly IDictionary<Type, IConsumerRegistration> _consumers;
        readonly IDictionary<Type, ISagaRegistration> _sagas;
        readonly IDictionary<Type, IExecuteActivityRegistration> _executeActivities;
        readonly IDictionary<Type, IActivityRegistration> _activities;

        public Registration(IConfigurationServiceProvider configurationServiceProvider, IDictionary<Type, IConsumerRegistration> consumers,
            IDictionary<Type, ISagaRegistration> sagas, IDictionary<Type, IExecuteActivityRegistration> executeActivities,
            IDictionary<Type, IActivityRegistration> activities)
        {
            _configurationServiceProvider = configurationServiceProvider;
            _consumers = consumers;
            _sagas = sagas;
            _executeActivities = executeActivities;
            _activities = activities;
        }

        public void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator)
        {
            if (_consumers.TryGetValue(consumerType, out var consumer))
                consumer.Configure(configurator, _configurationServiceProvider);
            else
                throw new ArgumentException($"The consumer type was not found: {TypeMetadataCache.GetShortName(consumerType)}", nameof(consumerType));
        }

        void IRegistration.ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
        {
            if (TryConfigureConsumer(configurator, configure))
                return;

            throw new ArgumentException($"The consumer type was not found: {TypeMetadataCache.GetShortName(typeof(T))}", nameof(T));
        }

        public bool TryConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer
        {
            if (_consumers.TryGetValue(typeof(T), out var consumer))
            {
                consumer.AddConfigureAction(configure);
                consumer.Configure(configurator, _configurationServiceProvider);

                return true;
            }

            return false;
        }

        public void ConfigureConsumers(IReceiveEndpointConfigurator configurator)
        {
            foreach (var consumer in _consumers.Values)
            {
                consumer.Configure(configurator, _configurationServiceProvider);
            }
        }

        public void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator)
        {
            if (_sagas.TryGetValue(sagaType, out var saga))
                saga.Configure(configurator, _configurationServiceProvider);
            else
                throw new ArgumentException($"The saga type was not found: {TypeMetadataCache.GetShortName(sagaType)}", nameof(sagaType));
        }

        void IRegistration.ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure)
        {
            if (TryConfigureSaga(configurator, configure))
                return;

            throw new ArgumentException($"The saga type was not found: {TypeMetadataCache.GetShortName(typeof(T))}", nameof(T));
        }

        public bool TryConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            if (_sagas.TryGetValue(typeof(T), out var saga))
            {
                saga.AddConfigureAction(configure);
                saga.Configure(configurator, _configurationServiceProvider);

                return true;
            }

            return false;
        }

        void IRegistration.ConfigureSagas(IReceiveEndpointConfigurator configurator)
        {
            foreach (var saga in _sagas.Values)
            {
                saga.Configure(configurator, _configurationServiceProvider);
            }
        }

        public void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
        {
            if (_executeActivities.TryGetValue(activityType, out var activity))
            {
                activity.Configure(configurator, _configurationServiceProvider);
            }
            else
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));
        }

        public void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (_activities.TryGetValue(activityType, out var activity))
            {
                activity.Configure(executeEndpointConfigurator, compensateEndpointConfigurator, _configurationServiceProvider);
            }
            else
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));
        }

        public void ConfigureEndpoints<T>(T configurator, IEndpointNameFormatter endpointNameFormatter)
            where T : IBusFactoryConfigurator
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (endpointNameFormatter == null)
                endpointNameFormatter = new DefaultEndpointNameFormatter();

            var consumersByEndpoint = _consumers.Values
                .Select(x => x.GetDefinition(_configurationServiceProvider))
                .GroupBy(x => x.GetEndpointName(endpointNameFormatter));

            var sagasByEndpoint = _sagas.Values
                .Select(x => x.GetDefinition(_configurationServiceProvider))
                .GroupBy(x => x.GetEndpointName(endpointNameFormatter));

            var activitiesByExecuteEndpoint = _activities.Values
                .Select(x => x.GetDefinition(_configurationServiceProvider))
                .GroupBy(x => x.GetExecuteEndpointName(endpointNameFormatter));

            var endpointNames = consumersByEndpoint.Select(x => x.Key)
                .Union(sagasByEndpoint.Select(x => x.Key))
                .Union(activitiesByExecuteEndpoint.Select(x => x.Key));

            var endpoints =
                from e in endpointNames
                join c in consumersByEndpoint on e equals c.Key into cs
                from c in cs.DefaultIfEmpty()
                join s in sagasByEndpoint on e equals s.Key into ss
                from s in ss.DefaultIfEmpty()
                join a in activitiesByExecuteEndpoint on e equals a.Key into aes
                from a in aes.DefaultIfEmpty()
                select new {Name = e, Consumers = c, Sagas = s, Activities = a};

            foreach (var endpoint in endpoints)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Configuring receive endpoint: {0}", endpoint.Name);

                configurator.ReceiveEndpoint(endpoint.Name, cfg =>
                {
                    if (endpoint.Consumers != null)
                        foreach (var consumer in endpoint.Consumers)
                        {
                            if (_log.IsDebugEnabled)
                                _log.DebugFormat("Configuring consumer {0} on {1}", TypeMetadataCache.GetShortName(consumer.ConsumerType), endpoint.Name);

                            ConfigureConsumer(consumer.ConsumerType, cfg);
                        }

                    if (endpoint.Sagas != null)
                        foreach (var saga in endpoint.Sagas)
                        {
                            if (_log.IsDebugEnabled)
                                _log.DebugFormat("Configuring saga {0} on {1}", TypeMetadataCache.GetShortName(saga.SagaType), endpoint.Name);

                            ConfigureSaga(saga.SagaType, cfg);
                        }

                    if (endpoint.Activities != null)
                        foreach (var activity in endpoint.Activities)
                        {
                            configurator.ReceiveEndpoint(activity.GetCompensateEndpointName(endpointNameFormatter), compensateEndpointConfigurator =>
                            {
                                ConfigureActivity(activity.ActivityType, cfg, compensateEndpointConfigurator);
                            });
                        }
                });
            }
        }
    }
}
