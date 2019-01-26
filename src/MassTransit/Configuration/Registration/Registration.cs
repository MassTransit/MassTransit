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
    using ConsumeConfigurators;
    using Saga;
    using Util;


    public class Registration :
        IRegistration
    {
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

        void IRegistration.ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
        {
            if (_executeActivities.TryGetValue(activityType, out var activity))
            {
                activity.Configure(configurator, _configurationServiceProvider);
            }
            else
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));
        }

        void IRegistration.ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            if (_activities.TryGetValue(activityType, out var activity))
            {
                activity.Configure(executeEndpointConfigurator, compensateEndpointConfigurator, _configurationServiceProvider);
            }
            else
                throw new ArgumentException($"The activity type was not found: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));
        }
    }
}
