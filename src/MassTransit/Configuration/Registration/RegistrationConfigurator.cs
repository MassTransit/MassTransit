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
    using System.Collections.Concurrent;
    using System.Linq;
    using ConsumeConfigurators;
    using Courier;
    using Util;


    /// <summary>
    /// Used for registration of consumers and sagas
    /// </summary>
    public class RegistrationConfigurator :
        IRegistrationConfigurator
    {
        readonly IContainerRegistrar _containerRegistrar;
        readonly ConcurrentDictionary<Type, IConsumerRegistration> _consumerConfigurations;
        readonly ConcurrentDictionary<Type, IExecuteActivityRegistration> _executeActivityRegistrations;
        readonly ConcurrentDictionary<Type, IActivityRegistration> _activityRegistrations;
        readonly ConcurrentDictionary<Type, ISagaRegistration> _sagaConfigurations;

        public RegistrationConfigurator(IContainerRegistrar containerRegistrar = null)
        {
            _containerRegistrar = containerRegistrar ?? new NullContainerRegistrar();

            _consumerConfigurations = new ConcurrentDictionary<Type, IConsumerRegistration>();
            _sagaConfigurations = new ConcurrentDictionary<Type, ISagaRegistration>();
            _executeActivityRegistrations = new ConcurrentDictionary<Type, IExecuteActivityRegistration>();
            _activityRegistrations = new ConcurrentDictionary<Type, IActivityRegistration>();
        }

        void IRegistrationConfigurator.AddConsumer<T>(Action<IConsumerConfigurator<T>> configure)
        {
            if (TypeMetadataCache<T>.HasSagaInterfaces)
                throw new ArgumentException($"{TypeMetadataCache<T>.ShortName} is a saga, and cannot be registered as a consumer", nameof(T));

            IConsumerRegistration ValueFactory(Type type)
            {
                ConsumerRegistrationCache.Register(type, _containerRegistrar);

                return new ConsumerRegistration<T>();
            }

            var configurator = _consumerConfigurations.GetOrAdd(typeof(T), ValueFactory);

            configurator.AddConfigureAction(configure);
        }

        void IRegistrationConfigurator.AddConsumer(Type consumerType, Type consumerDefinitionType)
        {
            if (TypeMetadataCache.HasSagaInterfaces(consumerType))
                throw new ArgumentException($"{TypeMetadataCache.GetShortName(consumerType)} is a saga, and cannot be registered as a consumer",
                    nameof(consumerType));

            IConsumerRegistration ValueFactory(Type type)
            {
                ConsumerRegistrationCache.Register(type, _containerRegistrar);

                if (consumerDefinitionType != null)
                    ConsumerDefinitionRegistrationCache.Register(consumerDefinitionType, _containerRegistrar);

                return (IConsumerRegistration)Activator.CreateInstance(typeof(ConsumerRegistration<>).MakeGenericType(type));
            }

            _consumerConfigurations.GetOrAdd(consumerType, ValueFactory);
        }

        void IRegistrationConfigurator.AddSaga<T>(Action<ISagaConfigurator<T>> configure)
        {
            ISagaRegistration ValueFactory(Type type)
            {
                SagaRegistrationCache.Register(type, _containerRegistrar);

                return new SagaRegistration<T>();
            }

            var configurator = _sagaConfigurations.GetOrAdd(typeof(T), ValueFactory);

            configurator.AddConfigureAction(configure);
        }

        void IRegistrationConfigurator.AddSaga<T>(SagaRegistrationFactory<T> factory, Action<ISagaConfigurator<T>> configure)
        {
            var configurator = _sagaConfigurations.GetOrAdd(typeof(T), _ => factory(_containerRegistrar));

            configurator.AddConfigureAction(configure);
        }

        void IRegistrationConfigurator.AddSaga(Type sagaType, Type sagaDefinitionType)
        {
            _sagaConfigurations.GetOrAdd(sagaType, type => SagaRegistrationCache.CreateRegistration(type, sagaDefinitionType, _containerRegistrar));
        }

        void IRegistrationConfigurator.AddExecuteActivity<TActivity, TArguments>(Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
        {
            IExecuteActivityRegistration ValueFactory(Type type)
            {
                ExecuteActivityRegistrationCache.Register(type, _containerRegistrar);

                return new ExecuteActivityRegistration<TActivity, TArguments>();
            }

            var configurator = _executeActivityRegistrations.GetOrAdd(typeof(TActivity), ValueFactory);

            configurator.AddConfigureAction(configure);
        }

        void IRegistrationConfigurator.AddExecuteActivity(Type activityType)
        {
            _executeActivityRegistrations.GetOrAdd(activityType, type => ExecuteActivityRegistrationCache.CreateRegistration(type, _containerRegistrar));
        }

        void IRegistrationConfigurator.AddActivity<TActivity, TArguments, TLog>(Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
        {
            IActivityRegistration ValueFactory(Type type)
            {
                ActivityRegistrationCache.Register(type, _containerRegistrar);

                return new ActivityRegistration<TActivity, TArguments, TLog>();
            }

            var registration = _activityRegistrations.GetOrAdd(typeof(TActivity), ValueFactory);

            registration.AddConfigureAction(configureExecute);
            registration.AddConfigureAction(configureCompensate);
        }

        public void AddActivity(Type activityType, Type activityDefinitionType)
        {
            _activityRegistrations.GetOrAdd(activityType,
                type => ActivityRegistrationCache.CreateRegistration(type, activityDefinitionType, _containerRegistrar));
        }

        public IRegistration CreateRegistration(IConfigurationServiceProvider configurationServiceProvider)
        {
            return new Registration(configurationServiceProvider, _consumerConfigurations.ToDictionary(x => x.Key, x => x.Value),
                _sagaConfigurations.ToDictionary(x => x.Key, x => x.Value), _executeActivityRegistrations.ToDictionary(x => x.Key, x => x.Value),
                _activityRegistrations.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
