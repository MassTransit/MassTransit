// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Autofac.Core;
    using AutofacIntegration;
    using Magnum.Extensions;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using SubscriptionConfigurators;

    public static class AutofacExtensions
    {
        public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, ILifetimeScope scope)
        {
            IList<Type> concreteTypes = FindTypes<IConsumer>(scope, r => !r.Implements<ISaga>());
            if (concreteTypes.Count != 0)
            {
                var consumerConfigurator = new AutofacConsumerFactoryConfigurator(configurator, scope);

                foreach (Type concreteType in concreteTypes)
                {
                    consumerConfigurator.ConfigureConsumer(concreteType);
                }
            }

            IList<Type> sagaTypes = FindTypes<ISaga>(scope, x => true);
            if (sagaTypes.Count > 0)
            {
                var sagaConfigurator = new AutofacSagaRepositoryFactoryConfigurator(configurator, scope);

                foreach (Type type in sagaTypes)
                {
                    sagaConfigurator.ConfigureSaga(type);
                }
            }
        }

        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
            this SubscriptionBusServiceConfigurator configurator, ILifetimeScope scope)
            where TConsumer : class
        {
            var consumerFactory = new AutofacConsumerFactory<TConsumer>(scope);

            return configurator.Consumer(consumerFactory);
        }

        public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(
            this SubscriptionBusServiceConfigurator configurator, ILifetimeScope scope)
            where TSaga : class, ISaga
        {
            return configurator.Saga(scope.Resolve<ISagaRepository<TSaga>>());
        }

        static IList<Type> FindTypes<T>(ILifetimeScope scope, Func<Type, bool> filter)
        {
            return scope.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new {r, s})
                .Where(rs => rs.s.ServiceType.Implements<T>())
                .Select(rs => rs.r.Activator.LimitType)
                .Where(filter)
                .ToList();
        }
    }
}