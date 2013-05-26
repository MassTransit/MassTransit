// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Magnum.Extensions;
    using Microsoft.Practices.Unity;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using SubscriptionConfigurators;
    using UnityIntegration;


    public static class UnityExtensions
    {
        public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, IUnityContainer container)
        {
            IList<Type> concreteTypes = FindTypes<IConsumer>(container, x => !x.Implements<ISaga>());
            if (concreteTypes.Count > 0)
            {
                var consumerConfigurator = new UnityConsumerFactoryConfigurator(configurator, container);

                foreach (Type concreteType in concreteTypes)
                    consumerConfigurator.ConfigureConsumer(concreteType);
            }

            IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
            if (sagaTypes.Count > 0)
            {
                var sagaConfigurator = new UnitySagaFactoryConfigurator(configurator, container);

                foreach (Type type in sagaTypes)
                    sagaConfigurator.ConfigureSaga(type);
            }
        }

        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
            this SubscriptionBusServiceConfigurator configurator, IUnityContainer container)
            where TConsumer : class, IConsumer
        {
            var consumerFactory = new UnityConsumerFactory<TConsumer>(container);

            return configurator.Consumer(consumerFactory);
        }

        public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(
            this SubscriptionBusServiceConfigurator configurator, IUnityContainer container)
            where TSaga : class, ISaga
        {
            var sagaRepository = container.Resolve<ISagaRepository<TSaga>>();

            var unitySagaRepository = new UnitySagaRepository<TSaga>(sagaRepository, container);

            return configurator.Saga(unitySagaRepository);
        }

        static IList<Type> FindTypes<T>(IUnityContainer container, Func<Type, bool> filter)
        {
            return container.Registrations
                            .Where(r => r.MappedToType.Implements<T>())
                            .Select(r => r.MappedToType)
                            .Where(filter)
                            .ToList();
        }
    }
}