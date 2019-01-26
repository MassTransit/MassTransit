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
    using Autofac;
    using AutofacIntegration;
    using AutofacIntegration.Registration;
    using ConsumeConfigurators;
    using Saga;


    /// <summary>
    /// Standard registration extensions, which are used to configure consumers, sagas, and activities on receive endpoints from a
    /// dependency injection container.
    /// </summary>
    public static class AutofacRegistrationExtensions
    {
        /// <summary>
        /// Adds the required services to the service collection, and allows consumers to be added and/or discovered
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        public static void AddMassTransit(this ContainerBuilder builder, Action<IContainerBuilderConfigurator> configure = null)
        {
            var configurator = new ContainerBuilderRegistrationConfigurator(builder);

            configure?.Invoke(configurator);
        }

        /// <summary>
        /// Configure a consumer (or consumers) on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="consumerTypes">The consumer type(s) to configure</param>
        public static void ConfigureConsumer(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope, params Type[] consumerTypes)
        {
            var registryConfigurator = lifetimeScope.Resolve<IRegistration>();

            foreach (var consumerType in consumerTypes)
            {
                registryConfigurator.ConfigureConsumer(consumerType, configurator);
            }
        }

        /// <summary>
        /// Configure a consumer on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="configure"></param>
        public static void ConfigureConsumer<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope,
            Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var registryConfigurator = lifetimeScope.Resolve<IRegistration>();

            registryConfigurator.ConfigureConsumer<T>(configurator, configure);
        }

        /// <summary>
        /// Configure all registered consumers on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        public static void ConfigureConsumers(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope)
        {
            var registryConfigurator = lifetimeScope.Resolve<IRegistration>();

            registryConfigurator.ConfigureConsumers(configurator);
        }

        /// <summary>
        /// Configure a saga (or sagas) on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="sagaTypes">The saga type(s) to configure</param>
        public static void ConfigureSaga(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope, params Type[] sagaTypes)
        {
            var registryConfigurator = lifetimeScope.Resolve<IRegistration>();

            foreach (var sagaType in sagaTypes)
            {
                registryConfigurator.ConfigureSaga(sagaType, configurator);
            }
        }

        /// <summary>
        /// Configure a saga on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="configure"></param>
        public static void ConfigureSaga<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var registryConfigurator = lifetimeScope.Resolve<IRegistration>();

            registryConfigurator.ConfigureSaga<T>(configurator, configure);
        }

        /// <summary>
        /// Configure all registered sagas on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        public static void ConfigureSagas(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope)
        {
            var registryConfigurator = lifetimeScope.Resolve<IRegistration>();

            registryConfigurator.ConfigureSagas(configurator);
        }

        /// <summary>
        /// Configure the execute activity on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="activityType"></param>
        public static void ConfigureExecuteActivity(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope, Type activityType)
        {
            var registryConfigurator = lifetimeScope.Resolve<IRegistration>();

            registryConfigurator.ConfigureExecuteActivity(activityType, configurator);
        }

        /// <summary>
        /// Configure an activity on two endpoints, one for execute, and the other for compensate
        /// </summary>
        /// <param name="executeEndpointConfigurator"></param>
        /// <param name="compensateEndpointConfigurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="activityType"></param>
        public static void ConfigureActivity(this IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator, ILifetimeScope lifetimeScope, Type activityType)
        {
            var registryConfigurator = lifetimeScope.Resolve<IRegistration>();

            registryConfigurator.ConfigureActivity(activityType, executeEndpointConfigurator, compensateEndpointConfigurator);
        }
    }
}
