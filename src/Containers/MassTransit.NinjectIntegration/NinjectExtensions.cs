// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Internals.Extensions;
    using Ninject;
    using NinjectIntegration;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using SubscriptionConfigurators;


    /// <summary>
    /// For NInject, it seems that using named scopes is the way to get per-message implementations
    /// http://www.planetgeek.ch/2010/12/08/how-to-use-the-additional-ninject-scopes-of-namedscope/
    /// </summary>
    public static class NinjectExtensions
    {
        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="kernel">The Ninject kernel.</param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IKernel kernel)
        {
            IList<Type> consumerTypes = FindTypes<IConsumer>(kernel, x => !x.HasInterface<ISaga>());
            if (consumerTypes.Count > 0)
            {
                foreach (Type type in consumerTypes)
                    ConsumerConfiguratorCache.Configure(type, configurator, kernel);
            }

            IList<Type> sagaTypes = FindTypes<ISaga>(kernel, x => true);
            if (sagaTypes.Count > 0)
            {
                foreach (Type sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, kernel);
            }
        }

        public static IConsumerConfigurator<TConsumer> Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator,
            IKernel kernel)
            where TConsumer : class, IConsumer
        {
            var consumerFactory = new NinjectConsumerFactory<TConsumer>(kernel);

            return configurator.Consumer(consumerFactory);
        }

        public static ISagaConfigurator<TSaga> Saga<TSaga>(this IReceiveEndpointConfigurator configurator, IKernel kernel)
            where TSaga : class, ISaga
        {
            var sagaRepository = kernel.Get<ISagaRepository<TSaga>>();

            var ninjectSagaRepository = new NinjectSagaRepository<TSaga>(sagaRepository, kernel);

            return configurator.Saga(ninjectSagaRepository);
        }

        static IList<Type> FindTypes<T>(IKernel kernel, Func<Type, bool> filter)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(typeof(T).IsAssignableFrom)
                .SelectMany(kernel.GetBindings)
                .Select(x => x.Service)
                .Distinct()
                .Where(filter)
                .ToList();
        }
    }
}