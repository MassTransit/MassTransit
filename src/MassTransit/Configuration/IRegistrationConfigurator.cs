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
namespace MassTransit
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using Definition;
    using Registration;
    using Saga;


    public interface IRegistrationConfigurator
    {
        /// <summary>
        /// Adds the consumer, allowing configuration when it is configured on an endpoint
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer;

        /// <summary>
        /// Adds the consumer, along with an optional consumer definition
        /// </summary>
        /// <param name="consumerType">The consumer type</param>
        /// <param name="consumerDefinitionType">The consumer definition type</param>
        void AddConsumer(Type consumerType, Type consumerDefinitionType = null);

        /// <summary>
        /// Adds the saga, allowing configuration when it is configured on the endpoint. This should not
        /// be used for state machine (Automatonymous) sagas.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        void AddSaga<T>(Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga;

        /// <summary>
        /// Adds the saga, using the specified factory, allowing configuration when it is configured on the endpoint. This is used
        /// to add advanced sagas, such as state machines, to the standard saga registration.
        /// </summary>
        /// <param name="factory">The saga registration factory</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type (or instance type, if it's a state machine)</typeparam>
        void AddSaga<T>(SagaRegistrationFactory<T> factory, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga;

        /// <summary>
        /// Adds the saga, along with an optional saga definition
        /// </summary>
        /// <param name="sagaType">The saga type</param>
        /// <param name="sagaDefinitionType">The saga definition type</param>
        void AddSaga(Type sagaType, Type sagaDefinitionType = null);

        /// <summary>
        /// Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        void AddExecuteActivity<TActivity, TArguments>(Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Adds an execute activity (Courier), along with an optional activity definition
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="activityDefinitionType"></param>
        void AddExecuteActivity(Type activityType, Type activityDefinitionType);

        /// <summary>
        /// Adds an activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configureExecute">The execute configuration callback</param>
        /// <param name="configureCompensate">The compensate configuration callback</param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        /// <typeparam name="TLog">The log type</typeparam>
        void AddActivity<TActivity, TArguments, TLog>(Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, Activity<TArguments, TLog>
            where TLog : class
            where TArguments : class;

        /// <summary>
        /// Adds an activity (Courier), along with an optional activity definition
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="activityDefinitionType"></param>
        void AddActivity(Type activityType, Type activityDefinitionType = null);

        /// <summary>
        /// Adds an endpoint definition, which will to used for consumers, sagas, etc. that are on that same endpoint. If a consumer, etc.
        /// specifies an endpoint without a definition, the default endpoint definition is used if one cannot be resolved from the configuration
        /// service provider (via generic registration).
        /// </summary>
        /// <param name="endpointDefinition">The endpoint definition to add</param>
        void AddEndpoint(Type endpointDefinition);

        void AddEndpoint<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings = null)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class;
    }
}
