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
    using Definition;
    using Util;


    /// <summary>
    /// Used for registration of consumers and sagas
    /// </summary>
    public class RegistrationConfigurator :
        IRegistrationConfigurator
    {
        readonly IContainerRegistrar _containerRegistrar;
        readonly ConcurrentDictionary<Type, IConsumerRegistration> _consumerRegistrations;
        readonly ConcurrentDictionary<Type, IExecuteActivityRegistration> _executeActivityRegistrations;
        readonly ConcurrentDictionary<Type, IActivityRegistration> _activityRegistrations;
        readonly ConcurrentDictionary<Type, ISagaRegistration> _sagaRegistrations;
        readonly ConcurrentDictionary<Type, IEndpointRegistration> _endpointRegistrations;

        public RegistrationConfigurator(IContainerRegistrar containerRegistrar = null)
        {
            _containerRegistrar = containerRegistrar ?? new NullContainerRegistrar();

            _consumerRegistrations = new ConcurrentDictionary<Type, IConsumerRegistration>();
            _sagaRegistrations = new ConcurrentDictionary<Type, ISagaRegistration>();
            _executeActivityRegistrations = new ConcurrentDictionary<Type, IExecuteActivityRegistration>();
            _activityRegistrations = new ConcurrentDictionary<Type, IActivityRegistration>();
            _endpointRegistrations = new ConcurrentDictionary<Type, IEndpointRegistration>();
        }

        IConsumerRegistrationConfigurator<T> IRegistrationConfigurator.AddConsumer<T>(Action<IConsumerConfigurator<T>> configure)
        {
            if (TypeMetadataCache<T>.HasSagaInterfaces)
                throw new ArgumentException($"{TypeMetadataCache<T>.ShortName} is a saga, and cannot be registered as a consumer", nameof(T));

            IConsumerRegistration ValueFactory(Type type)
            {
                ConsumerRegistrationCache.Register(type, _containerRegistrar);

                return new ConsumerRegistration<T>();
            }

            var registration = _consumerRegistrations.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            return new ConsumerRegistrationConfigurator<T>(this, registration, _containerRegistrar);
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

            _consumerRegistrations.GetOrAdd(consumerType, ValueFactory);
        }

        ISagaRegistrationConfigurator<T> IRegistrationConfigurator.AddSaga<T>(Action<ISagaConfigurator<T>> configure)
        {
            ISagaRegistration ValueFactory(Type type)
            {
                SagaRegistrationCache.Register(type, _containerRegistrar);

                return new SagaRegistration<T>();
            }

            var registration = _sagaRegistrations.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            return new SagaRegistrationConfigurator<T>(this, registration, _containerRegistrar);
        }

        ISagaRegistrationConfigurator<T> IRegistrationConfigurator.AddSaga<T>(SagaRegistrationFactory<T> factory, Action<ISagaConfigurator<T>> configure)
        {
            var registration = _sagaRegistrations.GetOrAdd(typeof(T), _ => factory(_containerRegistrar));

            registration.AddConfigureAction(configure);

            return new SagaRegistrationConfigurator<T>(this, registration, _containerRegistrar);
        }

        void IRegistrationConfigurator.AddSaga(Type sagaType, Type sagaDefinitionType)
        {
            _sagaRegistrations.GetOrAdd(sagaType, type => SagaRegistrationCache.CreateRegistration(type, sagaDefinitionType, _containerRegistrar));
        }

        IExecuteActivityRegistrationConfigurator<TActivity, TArguments> IRegistrationConfigurator.AddExecuteActivity<TActivity, TArguments>(
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
        {
            IExecuteActivityRegistration ValueFactory(Type type)
            {
                ExecuteActivityRegistrationCache.Register(type, _containerRegistrar);

                return new ExecuteActivityRegistration<TActivity, TArguments>();
            }

            var registration = _executeActivityRegistrations.GetOrAdd(typeof(TActivity), ValueFactory);

            registration.AddConfigureAction(configure);

            return new ExecuteActivityRegistrationConfigurator<TActivity, TArguments>(this, registration, _containerRegistrar);
        }

        void IRegistrationConfigurator.AddExecuteActivity(Type activityType, Type activityDefinitionType)
        {
            _executeActivityRegistrations.GetOrAdd(activityType,
                type => ExecuteActivityRegistrationCache.CreateRegistration(type, activityDefinitionType, _containerRegistrar));
        }

        IActivityRegistrationConfigurator<TActivity, TArguments, TLog> IRegistrationConfigurator.AddActivity<TActivity, TArguments, TLog>(
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute,
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

            return new ActivityRegistrationConfigurator<TActivity, TArguments, TLog>(this, registration, _containerRegistrar);
        }

        public void AddActivity(Type activityType, Type activityDefinitionType)
        {
            _activityRegistrations.GetOrAdd(activityType,
                type => ActivityRegistrationCache.CreateRegistration(type, activityDefinitionType, _containerRegistrar));
        }

        public void AddEndpoint(Type definitionType)
        {
            _endpointRegistrations.GetOrAdd(definitionType, type => EndpointRegistrationCache.CreateRegistration(definitionType, _containerRegistrar));
        }

        public void AddEndpoint<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            IEndpointRegistration ValueFactory(Type type)
            {
                _containerRegistrar.RegisterEndpointDefinition<TDefinition, T>(settings);

                return new EndpointRegistration<T>();
            }

            _endpointRegistrations.GetOrAdd(typeof(TDefinition), ValueFactory);
        }

        public IRegistration CreateRegistration(IConfigurationServiceProvider configurationServiceProvider)
        {
            return new Registration(configurationServiceProvider, _consumerRegistrations.ToDictionary(x => x.Key, x => x.Value),
                _sagaRegistrations.ToDictionary(x => x.Key, x => x.Value), _executeActivityRegistrations.ToDictionary(x => x.Key, x => x.Value),
                _activityRegistrations.ToDictionary(x => x.Key, x => x.Value), _endpointRegistrations.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
